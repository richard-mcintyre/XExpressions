using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;

namespace XExpressions
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

            Node BuildGraphForNode(ExpressionNode treeNode)
            {
                Node p = graph.AddNode(Guid.NewGuid().ToString());

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

                p.Label.Width = 100;
                p.Label.Height = 1;
                p.LabelText = $"{treeNode.Token.Kind}: {treeNode.Token.Value}";

                foreach (ExpressionNode childNode in treeNode.Children)
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
                n.GeometryNode.BoundaryCurve = CurveFactory.CreateRectangleWithRoundedCorners(150, 50, 3, 2, new Microsoft.Msagl.Core.Geometry.Point(0, 0));
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

    }
}
