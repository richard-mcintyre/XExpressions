using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;

namespace XExpressions.SVG
{
    public static class TreeNodeExtensions
    {
        /// <summary>
        /// Generates an SVG from the specified tree representing an expression
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static string CreateSvg(this ExpressionNode rootNode)
        {
            Graph graph = new Graph();

            HashSet<string> numberIds = new HashSet<string>();

            Node BuildGraphForNode(ExpressionNode treeNode)
            {
                string id = Guid.NewGuid().ToString();
                Node p = graph.AddNode(id);

                switch (treeNode.Token.Kind)
                {
                    case TokenKind.AddOperator:
                    case TokenKind.SubtractOperator:
                    case TokenKind.MultiplyOperator:
                    case TokenKind.DivideOperator:
                    case TokenKind.Equals:
                        //p.Attr.FillColor = Color.Red;
                        break;

                    case TokenKind.Identifier:
                        p.Attr.FillColor = Color.LightBlue;
                        break;

                    case TokenKind.Number:
                    case TokenKind.String:
                    case TokenKind.Boolean:
                        p.Attr.FillColor = Color.LightGreen;
                        break;

                    case TokenKind.Function:
                        p.Attr.FillColor = Color.LightPink;
                        break;
                }

                if (treeNode.Token.Kind == TokenKind.Number)
                    numberIds.Add(id);

                p.Label.Width = (treeNode.Token.Kind == TokenKind.Number) ? 25 : 75;
                p.Label.Height = 15;
                p.LabelText = GetLabelTextForToken(treeNode.Token);

                foreach (ExpressionNode childNode in treeNode.Children.Reverse<ExpressionNode>())
                {
                    Node c = BuildGraphForNode(childNode);
                    graph.AddEdge(p.Id, c.Id);
                }

                return p;
            }

            BuildGraphForNode(rootNode);


            graph.CreateGeometryGraph();

            foreach (Node n in graph.Nodes)
            {
                double width = 150;
                if (numberIds.Contains(n.Id))
                    width = 50;

                n.GeometryNode.BoundaryCurve = CurveFactory.CreateRectangleWithRoundedCorners(width, 30, 3, 2, new Microsoft.Msagl.Core.Geometry.Point(0, 0));
            }

            SugiyamaLayoutSettings layoutSettings = new SugiyamaLayoutSettings();
            layoutSettings.GridSizeByX = 10;
            layoutSettings.GridSizeByY = 10;

            LayoutHelpers.CalculateLayout(graph.GeometryGraph, layoutSettings, null);

            using (MemoryStream ms = new MemoryStream())
            {
                SvgGraphWriter writer = new SvgGraphWriter(ms, graph);
                writer.Write();


                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static string GetLabelTextForToken(Token token)
        {
            switch (token.Kind)
            {
                case TokenKind.Number:
                    return token.Value;

                case TokenKind.String:
                    return $"\"{token.Value}\"";

                case TokenKind.Boolean:
                    return Convert.ToBoolean(token.Value) ? Boolean.TrueString : Boolean.FalseString;

                case TokenKind.Identifier:
                    return $"Identifier: {token.Value}";

                case TokenKind.AddOperator:
                    return "add";

                case TokenKind.SubtractOperator:
                    return "subtract";

                case TokenKind.MultiplyOperator:
                    return "multiply";

                case TokenKind.DivideOperator:
                    return "divide";

                case TokenKind.Equals:
                    return "equals";

                case TokenKind.Function:
                    return $"Function: {token.Value}";
            }

            return $"{token.Kind}: {token.Value}";
        }
    }
}