using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileGameLib
{
    public class Pathfinder
    {
        int levelWidth, levelHeight;
        TileLayer layer;

        SearchNode[,] searchNodes;
        List<SearchNode> openList = new List<SearchNode>();
        List<SearchNode> closedList = new List<SearchNode>();

        #region Initialization

        public Pathfinder(TileLayer layer)
        {
            levelWidth = layer.Width();
            levelHeight = layer.Height();
            this.layer = layer;
            InitializeSearchNodes(layer);
        }

        public void InitializeSearchNodes(TileLayer layer)
        {
            searchNodes = new SearchNode[levelWidth, levelHeight];

            for (int x = 0; x < levelWidth; ++x)
            {
                for (int y = 0; y < levelHeight; ++y)
                {
                    SearchNode node = new SearchNode();

                    node.Position = new Point(x, y);

                    node.Passable = layer.IsPassable(x, y);

                    if (node.Passable)
                    {
                        node.Neighbors = new SearchNode[4];

                        searchNodes[x, y] = node;
                    }
                }
            }

            for (int x = 0; x < levelWidth; ++x)
            {
                for (int y = 0; y < levelHeight; ++y)
                {
                    SearchNode node = searchNodes[x, y];

                    if (node == null || !node.Passable)
                        continue;

                    Point[] neighbors = new Point[]
                    {
                        new Point(x, y - 1),
                        new Point(x, y + 1),
                        new Point(x - 1, y),
                        new Point(x + 1, y)
                    };

                    for (int i = 0; i < neighbors.Length; ++i)
                    {
                        Point position = neighbors[i];

                        if (position.X < 0 || position.X >= levelWidth || position.Y < 0 || position.Y >= levelHeight)
                            continue;

                        SearchNode neighbor = searchNodes[position.X, position.Y];

                        if (neighbor == null || !neighbor.Passable)
                            continue;

                        node.Neighbors[i] = neighbor;
                    }
                }
            }
        }

        #endregion

        #region Algorithm

        public List<Point> FindPath(Point startPoint, Point endPoint)
        {
            if (startPoint == endPoint)
                return new List<Point>();

            resetSearchNodes();

            SearchNode startNode = searchNodes[startPoint.X, startPoint.Y];
            SearchNode endNode = searchNodes[endPoint.X, endPoint.Y];

            startNode.InOpenList = true;

            startNode.DistanceToGoal = heuristic(startPoint, endPoint);
            startNode.DistanceTraveled = 0;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                SearchNode currentNode = findBestNode();

                if (currentNode == null)
                    break;

                if (currentNode == endNode)
                    return findFinalPath(startNode, endNode);

                for (int i = 0; i < currentNode.Neighbors.Length; ++i)
                {
                    SearchNode neighbor = currentNode.Neighbors[i];

                    if (neighbor == null || !neighbor.Passable || (layer.Entities.EntityAt(neighbor.Position) != null && layer.Entities.EntityAt(neighbor.Position).Group != "Player"))
                        continue;

                    float distanceTraveled = currentNode.DistanceTraveled + (layer.IsSlowTile(neighbor.Position) ? 2 : 1);
                    float heur = heuristic(neighbor.Position, endPoint);

                    if (!neighbor.InOpenList && !neighbor.InClosedList)
                    {
                        neighbor.DistanceTraveled = distanceTraveled;
                        neighbor.DistanceToGoal = distanceTraveled + heur;

                        neighbor.Parent = currentNode;
                        neighbor.InOpenList = true;
                        openList.Add(neighbor);
                    }

                    else if (neighbor.InOpenList || neighbor.InClosedList)
                    {
                        if (neighbor.DistanceTraveled > distanceTraveled)
                        {
                            neighbor.DistanceTraveled = distanceTraveled;
                            neighbor.DistanceToGoal = distanceTraveled + heur;

                            neighbor.Parent = currentNode;
                        }
                    }
                }

                openList.Remove(currentNode);
                currentNode.InClosedList = true;
            }

            return new List<Point>();
        }

        #endregion

        #region Algorithm Helpers

        private float heuristic(Point p1, Point p2)
        {
            return (float)(Math.Sqrt(Math.Pow(Math.Abs(p1.X - p2.X), 2) + Math.Pow(Math.Abs(p1.Y - p2.Y), 2)));
        }

        public int AttackDistance(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        public int MoveDistance(Point p1, Point p2)
        {
            int sum = 0;
            List<Point> path = FindPath(p1, p2);
            if (path.Count == 0)
                return int.MaxValue;
            foreach (Point p in FindPath(p1, p2))
            {
                if (layer.IsSlowTile(p))
                    sum += 2;
                else
                    ++sum;
            }
            return sum;
        }

        private List<Point> findFinalPath(SearchNode startNode, SearchNode endNode)
        {
            closedList.Add(endNode);

            SearchNode parentNode = endNode.Parent;

            while (parentNode != startNode)
            {
                closedList.Add(parentNode);
                parentNode = parentNode.Parent;
            }

            List<Point> finalPath = new List<Point>();

            for (int i = closedList.Count - 1; i >= 0; --i)
            {
                finalPath.Add(closedList[i].Position);
            }

            return finalPath;
        }

        private SearchNode findBestNode()
        {
            SearchNode currentNode = openList[0];

            float smallestDistanceToGoal = float.MaxValue;

            for (int i = 0; i < openList.Count; ++i)
            {
                if (openList[i].DistanceToGoal < smallestDistanceToGoal)
                {
                    currentNode = openList[i];
                    smallestDistanceToGoal = openList[i].DistanceToGoal;
                }
            }

            return currentNode;
        }

        #endregion

        #region Helpers

        private void resetSearchNodes()
        {
            openList.Clear();
            closedList.Clear();

            for (int x = 0; x < levelWidth; ++x)
            {
                for (int y = 0; y < levelHeight; ++y)
                {
                    SearchNode node = searchNodes[x, y];

                    if (node == null)
                        continue;

                    node.InOpenList = false;
                    node.InClosedList = false;

                    node.DistanceTraveled = float.MaxValue;
                    node.DistanceToGoal = float.MaxValue;
                }
            }
        }

        #endregion
    }
}
