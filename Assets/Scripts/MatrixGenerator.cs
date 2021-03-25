using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LevelGenerator
{
    public class Pair<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
        public Pair(T first, T second)
        {
            X = first;
            Y = second;
        }

        public static bool operator ==(Pair<T> pair1, Pair<T> pair2)
        {
            return pair1.X.Equals(pair2.X) && pair1.Y.Equals(pair2.Y);
        }
        public static bool operator !=(Pair<T> pair1, Pair<T> pair2)
        {
            return !(pair1.X.Equals(pair2.X) && pair1.Y.Equals(pair2.Y));
        }
        public override bool Equals(object obj)
        {
            if (obj is Pair<T>)
            {
                Pair<T> pair = obj as Pair<T>;
                if (pair.X.Equals(X) && pair.Y.Equals(Y))
                {
                    return true;
                }
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public enum TileType
    {
        Empty,
        Breakable,
        Unbreakable,
    }
    public class Tile
    {
        public Pair<int> Position { get; set; }
        public bool IsDisturbed { get; set; }
        public Tile()
        {
            Position = new Pair<int>(0, 0);
            Type = TileType.Unbreakable;
            IsDisturbed = false;
        }

        public Tile(int x, int y)
        {
            Position = new Pair<int>(x, y);
            Type = TileType.Unbreakable;
            IsDisturbed = false;
        }

        public void Break()
        {
            Type = TileType.Empty;
        }

        public void Disturbe()
        {
            Type = TileType.Empty;
            IsDisturbed = true;
        }

        public string ToChar()
        {
            if (Type == TileType.Empty)
            {
                return " ";
            }
            else
            {
                return "o";
            }
        }
        public TileType Type { get; set; }
    }

    private readonly Tile[,] tiles_;
    private readonly List<Tile> disturbed_;
    private List<Tile> empty_;
    public int Heigth { get; }
    public int Width { get; }
    public List<Tile> EmptyTiles
    {
        get
        {
            return empty_;
        }
    }

    public Tile GetRandomTile()
    {
        int randomIndex = UnityEngine.Random.Range(0, empty_.Count);
        Tile temp = empty_[randomIndex];
        empty_.RemoveAt(randomIndex);
        return temp;
    }

    public LevelGenerator(int x, int y)
    {
        tiles_ = new Tile[x, y];
        Heigth = x;
        Width = y;
        disturbed_ = new List<Tile>();
        for (int i = 0; i < Heigth; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                tiles_[i, j] = new Tile(i, j);
            }
        }
        empty_ = new List<Tile>();
        Build();
    }

    public bool IsWall(int x, int y)
    {
        if (tiles_[x, y].Type != TileType.Empty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Disturbe()
    {
        Random rng = new Random();
        for (int i = 0; i < Math.Ceiling(0.02f * (Heigth - 1) * (Width - 1)); i++)
        {
            int randX = rng.Next(Heigth);
            int randY = rng.Next(Width);
            tiles_[randX, randY].Disturbe();
            disturbed_.Add(tiles_[randX, randY]);
        }
    }

    private void Line(Pair<int> tile1, Pair<int> tile2)
    {
        int dHeigth = Math.Abs(tile1.X - tile2.X);
        int dWidth = Math.Abs(tile1.Y - tile2.Y);
        if (dHeigth == 0)
        {
            for (int i = Math.Min(tile1.Y, tile2.Y); i <= Math.Max(tile1.Y, tile2.Y); i++)
            {
                tiles_[tile1.X, i].Break();
            }
            return;
        }
        if (dWidth == 0)
        {
            for (int i = Math.Min(tile1.X, tile2.X); i <= Math.Max(tile1.X, tile2.X); i++)
            {
                tiles_[i, tile1.Y].Break();
            }
            return;
        }
        throw new ArgumentException("Tiles cannot be connected by a straight line.");
    }

    private void Connect(Tile tile1, Tile tile2)
    {
        if (tile1.IsDisturbed && tile2.IsDisturbed)
        {
            int dHeigth = -tile1.Position.X + tile2.Position.X;
            int dWidth = -tile1.Position.Y + tile2.Position.Y;
            if (dHeigth == 0 || dWidth == 0)
            {
                Line(tile1.Position, tile2.Position);
                return;
            }
            if (Math.Abs(dHeigth) > Math.Abs(dWidth))
            {
                int curX = tile1.Position.X;
                int curY = tile1.Position.Y;
                int correction = dHeigth % Math.Abs(dWidth) - Math.Sign(dHeigth);
                for (int i = 0; i < Math.Abs(dWidth); i++)
                {
                    Line(new Pair<int>(curX, curY), new Pair<int>(curX + dHeigth / Math.Abs(dWidth), curY));
                    curX += dHeigth / Math.Abs(dWidth);
                    Line(new Pair<int>(curX, curY), new Pair<int>(curX, curY + Math.Sign(dWidth)));
                    curY += Math.Sign(dWidth);
                }
                Line(new Pair<int>(curX, curY), new Pair<int>(curX + correction, curY));
            }
            else
            {
                int curX = tile1.Position.X;
                int curY = tile1.Position.Y;
                int correction = dWidth % Math.Abs(dHeigth) - Math.Sign(dWidth);
                for (int i = 0; i < Math.Abs(dHeigth); i++)
                {
                    Line(new Pair<int>(curX, curY), new Pair<int>(curX + Math.Sign(dHeigth), curY));
                    curX += Math.Sign(dHeigth);

                    Line(new Pair<int>(curX, curY), new Pair<int>(curX, curY + dWidth / Math.Abs(dHeigth)));
                    curY += dWidth / Math.Abs(dHeigth);
                }
                Line(new Pair<int>(curX, curY), new Pair<int>(curX, curY + correction));
            }
        }
    }

    private void Merge()
    {
        Tile[] tiles = disturbed_.ToArray();
        for (int i = 0; i < tiles.Length - 1; i++)
        {
            Connect(tiles[i], tiles[i + 1]);
        }
    }
    private void Explode(Tile tile, int radius)
    {
        int curX = (tile.Position.X - radius >= 0) ? tile.Position.X - radius : 0;
        int curY = tile.Position.Y;
        for (int i = curX; i <= ((curX + radius * 2 < Heigth) ? curX + radius * 2 : Heigth - 1); i++)
        {
            for (int j = (curY - radius >= 0) ? curY - radius : 0; j <= ((curY + radius < Width) ? curY + radius : Width - 1); j++)
            {
                tiles_[i, j].Break();
            }
        }
    }
    public void Build()
    {
        Disturbe();
        Merge();
        foreach (var tile in disturbed_)
        {
            Explode(tile, Convert.ToInt32(Math.Ceiling(0.01 * Math.Min(Heigth, Width))));
        }

        for (int i = 0; i < Heigth; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (tiles_[i, j].Type == TileType.Empty)
                {
                    empty_.Add(tiles_[i, j]);
                }
            }
        }
    }
    public void ConsolePrint()
    {
        for (int i = 0; i < Heigth; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Console.Write(tiles_[i, j].ToChar());
            }
            Console.Write("\n");
        }
        Console.WriteLine("\n");
    }
}
