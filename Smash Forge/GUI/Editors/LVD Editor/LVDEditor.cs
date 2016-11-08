﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Smash_Forge
{
    public partial class LVDEditor : DockContent
    {
        public LVDEditor()
        {
            InitializeComponent();
        }

        private LVDEntry currentEntry;
        private Vector2D currentVert;
        private Vector2D currentNormal;
        private CollisionMat currentMat;
        private TreeNode currentTreeNode;
        private Point currentPoint;

        enum materialTypes : byte
        {
            Iron = 0x06,
            Snow = 0x0d,
            Ice = 0x0c,
            Wood = 0x04,
            LargeBubbles = 0x15,
            Hurt = 0x1f,
            Brick = 0x00,
            Stone2 = 0x18,
            Metal2 = 0x1b,
            Water = 0x0a,
            Bubbles = 0x0b,
            Clouds = 0x16,
            Ice2 = 0x10,
            NebuIron = 0x05,
            Danbouru = 0x11,
            Rock = 0x01,
            Gamewatch = 0x0f,
            Grass = 0x02,
            SnowIce = 0x0e,
            Fence = 0x08,
            Soil = 0x03,
            Sand = 0x1c,
            MasterFortress = 0x09,
            Carpet = 0x07
        }

        public void open(LVDEntry entry, TreeNode entryTree)
        {
            currentTreeNode = entryTree;
            currentEntry = entry;
            collisionGroup.Visible = false;
            pointGroup.Visible = false;
            name.Text = currentEntry.name;
            subname.Text = currentEntry.subname;
            if (entry is Collision)
            {
                Collision col = (Collision)entry;
                collisionGroup.Visible = true;
                xStart.Value = (decimal)col.unk1[0];
                yStart.Value = (decimal)col.unk1[1];
                zStart.Value = (decimal)col.unk1[2];
                flag1.Checked = col.useStartPos;
                flag2.Checked = col.flag2;
                flag3.Checked = col.flag3;
                flag4.Checked = col.flag4;
                vertices.Nodes.Clear();
                for (int i = 0; i < col.verts.Count; i++)
                    vertices.Nodes.Add(new TreeNode($"Vertex {i}") { Tag = col.verts[i] });
                lines.Nodes.Clear();
                for (int i = 0; i < col.normals.Count; i++)
                {
                    object[] temp = { col.normals[i], col.materials[i] };
                    lines.Nodes.Add(new TreeNode($"Line {i}") { Tag = temp });
                }
            }
            else if(entry is Point)
            {
                pointGroup.Visible = true;
                currentPoint = (Point)entry;
                xPoint.Value = (decimal)((Point)entry).x;
                yPoint.Value = (decimal)((Point)entry).y;
            }
        }

        private void vertices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currentVert = (Vector2D)e.Node.Tag;
            xVert.Value = (decimal)currentVert.x;
            yVert.Value = (decimal)currentVert.y;
        }

        private void lines_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currentNormal = (Vector2D)((object[])e.Node.Tag)[0];
            currentMat = (CollisionMat)((object[])e.Node.Tag)[1];
            leftLedge.Checked = currentMat.getFlag(2);
            rightLedge.Checked = currentMat.getFlag(3);
            noWallJump.Checked = currentMat.getFlag(1);
            comboBox1.Text = Enum.GetName(typeof(materialTypes), currentMat.getPhysics());
            passthroughAngle.Value = (decimal)(Math.Atan2(currentNormal.y, currentNormal.x) * 180.0 / Math.PI);
        }

        private void flagChange(object sender, EventArgs e)
        {
            if(sender == flag1)
                ((Collision)currentEntry).useStartPos = flag1.Checked;
            if (sender == flag2)
                ((Collision)currentEntry).flag2 = flag2.Checked;
            if (sender == flag3)
                ((Collision)currentEntry).flag3 = flag3.Checked;
            if (sender == flag4)
                ((Collision)currentEntry).flag4 = flag4.Checked;
        }

        private void changePos(object sender, EventArgs e)
        {
            if(sender == xVert)
                currentVert.x = (float)xVert.Value;
            if(sender == yVert)
                currentVert.y = (float)yVert.Value;
        }

        private void nameChange(object sender, EventArgs e)
        {
            if (sender == name)
            {
                currentEntry.name = name.Text;
                currentTreeNode.Text = name.Text;
            }
                
            if (sender == subname)
                currentEntry.subname = subname.Text;
        }

        private void passthroughAngle_ValueChanged(object sender, EventArgs e)
        {
            double theta = (double)((NumericUpDown)sender).Value;
            currentNormal.x = (float)Math.Cos(theta * Math.PI / 180.0f);
            currentNormal.y = (float)Math.Sin(theta * Math.PI / 180.0f);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             currentMat.setPhysics((byte)Enum.Parse(typeof(materialTypes), comboBox1.Text));
        }

        private void changeStart(object sender, EventArgs e)
        {
            if (sender == xStart)
                ((Collision)currentEntry).unk1[0] = (float)xStart.Value;
            if (sender == yStart)
                ((Collision)currentEntry).unk1[1] = (float)yStart.Value;
            if (sender == zStart)
                ((Collision)currentEntry).unk1[2] = (float)zStart.Value;
        }

        private void lineFlagChange(object sender, EventArgs e)
        {
            if (sender == rightLedge)
                currentMat.setFlag(3, ((CheckBox)sender).Checked);
            if (sender == leftLedge)
                currentMat.setFlag(2, ((CheckBox)sender).Checked);
            if (sender == noWallJump)
                currentMat.setFlag(1, ((CheckBox)sender).Checked);
        }

        private void LVDEditor_Load(object sender, EventArgs e)
        {
            collisionGroup.Visible = false;
            pointGroup.Visible = false;
        }

        private void pointMoved(object sender, EventArgs e)
        {
            if (sender == xPoint)
                currentPoint.x = (float)xPoint.Value;
            if (sender == yPoint)
                currentPoint.y = (float)yPoint.Value;
            Console.WriteLine("(" + currentPoint.x + "," + currentPoint.y + ")");
        }
    }
}
