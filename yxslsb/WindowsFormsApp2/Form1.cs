using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public int wkey,rkey;
        SynchronizationContext m_SyncContext = null;
        string t = "";
        int le;
        public Form1()
        {
            InitializeComponent();
            m_SyncContext = SynchronizationContext.Current;
        }
        private void setrichtext(Object o)
        {
            richTextBox2.Text = o.ToString();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void SetTextSafePost(object text)
        {
            toolStripStatusLabel1.Text = text.ToString();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image i = Image.FromFile(ofd.FileName);
                if (i.Width < 100 && i.Height < 100)
                {
                    MessageBox.Show("图片太小","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                else
                {
                    pictureBox1.Image = i.Clone() as Image;
                    if (((i.Width - 1) * (i.Height - 1) * 3 / 8) < 10000000)
                    {
                        richTextBox1.MaxLength = (i.Width - 1) * (i.Height - 1) * 3 / 16;
                    }
                    else
                    {
                        richTextBox1.MaxLength = 8000000;
                    }
                    label6.Text = "内容（支持写入" + richTextBox1.MaxLength + "个字符）:";
                    i.Dispose();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "开始";
            le = richTextBox1.Text.Length;
            t = richTextBox1.Text;
            if (!(pictureBox1.Image == null))
            {
                if (t != "")
                {
                    if (maskedTextBox2.Text.Equals(""))
                    {
                        
                        Thread thread = new Thread(write);
                        thread.Name = "write";

                        thread.Start();
                    }
                    else
                    {
                        wkey = Convert.ToInt32(maskedTextBox2.Text);
                        Thread thread = new Thread(safewrite);
                        thread.Start();
                    }
                }
                else
                {
                    MessageBox.Show("内容为空","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("图片为空","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }
        private void write()
        {
            Bitmap bitmap = pictureBox1.Image.Clone() as Bitmap;
            char[] c = t.ToCharArray(); 
            StringBuilder ab=new StringBuilder();
            int i = 0;
            foreach (char oc in c)
            {
                string by = Convert.ToString(oc, 2).PadLeft(16,'0');
                ab.Append(by);
            }
            for (int x = 0; x < bitmap.Width-1; x++)
            {
                for (int y = 0; y < bitmap.Height-1; y++)
                {
                    if (i < ab.Length)
                    {
                        Color pixel = bitmap.GetPixel(x, y);
                        string rt, gt, bt;
                        int r, g, b;
                        r = pixel.R;
                        g = pixel.G;
                        b = pixel.B;
                        rt = Convert.ToString(pixel.R, 2).PadLeft(8, '0');
                        gt = Convert.ToString(pixel.G, 2).PadLeft(8, '0');
                        bt = Convert.ToString(pixel.B, 2).PadLeft(8, '0');
                        if (rt[7] != ab[i])
                        {
                            if (r < 255)
                            {
                                r += 1;
                            }
                            else
                            {
                                r -= 1;
                            }
                            
                        }
                        i += 1;
                        if (i < ab.Length)
                        {
                            if (gt[7] != ab[i])
                            {
                                if (g < 255)
                                {
                                    g += 1;
                                }
                                else
                                {
                                    g -= 1;
                                }
                                
                            }
                            i += 1;
                        }
                        if (i < ab.Length)
                        {
                            if (bt[7] != ab[i])
                            {
                                if (b < 255)
                                {
                                    b += 1;
                                }
                                else
                                {
                                    b -= 1;
                                }
                                
                            }
                            i += 1;
                        }
                        bitmap.SetPixel(x, y, Color.FromArgb(pixel.A,r, g, b));
                    }
                    else
                    {
                        break;
                    }
                }
                

                if (i > ab.Length)
                {
                    break;
                }
            }
            string left = Convert.ToString(le,2).PadLeft(24,'0');
            for (int ilf = 0; ilf < left.Length; ilf++)
            {

                Color color = bitmap.GetPixel(bitmap.Width-1, 0);
                string rt = Convert.ToString(color.R, 2).PadLeft(8, '0');
                int r = color.R;
                if (left[ilf] != Convert.ToString(r, 2).PadLeft(8,'0')[7])
                {
                    if (r < 255)
                    {
                        r += 1;
                    }
                    else
                    {
                        r -= 1;
                    }
                }
                bitmap.SetPixel(bitmap.Width-1, ilf ,Color.FromArgb(color.A,r,color.G,color.B));
            }
            m_SyncContext.Post(SetTextSafePost, "写入完成");
            pictureBox2.Image = bitmap.Clone() as Image;
            bitmap.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch(Exception et)
            {
                MessageBox.Show(et.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error) ;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image i = Image.FromFile(ofd.FileName);
                if (i.Width < 100 && i.Height < 100)
                {
                    MessageBox.Show("图片太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    pictureBox3.Image = i.Clone() as Image;
                    i.Dispose();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            if (!(pictureBox3.Image == null))
            {
                if (maskedTextBox1.Text.Equals(""))
                {
                    Thread thread = new Thread(read);
                    thread.Start();
                }
                else
                {
                    rkey = Convert.ToInt32(maskedTextBox1.Text);
                    Thread thread = new Thread(saferead);
                    thread.Start();
                }
            }
            else
            {
                MessageBox.Show("图片为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void read()
        {
            int i = 0;
            int il = 0;
            int leght = 0;
            string lb = "";
            
            StringBuilder ab = new StringBuilder();
            Bitmap bitmap = pictureBox3.Image.Clone() as Bitmap;
            for (int ik = 0; ik < 24; ik++)
            {
                Color c = bitmap.GetPixel(bitmap.Width-1, ik);
                char cb = Convert.ToString(c.R, 2).PadLeft(8,'0')[7];
                lb += cb;
            }
            leght = Convert.ToInt32(lb, 2);
            for (int x = 0; x < bitmap.Width-1; x++)
            {
                for (int y = 0; y < bitmap.Height-1; y++)
                {
                    
                    Color color = bitmap.GetPixel(x, y);

                    string r = Convert.ToString(color.R, 2).PadLeft(8, '0');
                    ab.Append(r[7]); 
                    i += 1;
                    if (i % 16 == 0 && i != 0)
                    {
                        
                        il += 1;
                        if (il >= leght)
                        {
                            break;
                        }
                        ab.Append(".");
                    }
                    string g = Convert.ToString(color.G, 2).PadLeft(8, '0');
                    ab.Append(g[7]);
                    i += 1;
                    if (i % 16 == 0 && i != 0)
                    {
                        
                        il += 1;
                        if (il >= leght)
                        {
                            break;
                        }
                        ab.Append(".");
                    }
                    string b = Convert.ToString(color.B, 2).PadLeft(8, '0');
                    ab.Append(b[7]);
                    i += 1;
                    if (i % 16 == 0 && i != 0)
                    {
                        
                        il += 1;
                        if (il >= leght)
                        {
                            break;
                        }
                        ab.Append(".");
                    }
                }
                if (il >= leght)
                {
                    break;
                }
            }
            string[] vs = ab.ToString().Split('.');
            StringBuilder t = new StringBuilder();
            int li = 0;
            foreach (string c in vs)
            {
                if (li <= leght)
                {
                    int a = Convert.ToInt32(c, 2);
                    char cs = Convert.ToChar(a);
                    t.Append(cs);
                }
                li += 1;
            }
            m_SyncContext.Post(setrichtext, t);
            m_SyncContext.Post(SetTextSafePost, "读取完成");

        }
        public void saferead()
        {
            int i = 0;
            int il = 0;
            int leght = 0;
            string lb = "";
            int seed = rkey;
            StringBuilder ab = new StringBuilder();
            Bitmap bitmap = pictureBox3.Image.Clone() as Bitmap;
            List <Point>  points= new List<Point>();
            Random random = new Random(seed);
            for (int ik = 0; ik < 24; ik++)
            {
                Color c = bitmap.GetPixel(bitmap.Width - 1, ik);
                char cb = Convert.ToString(c.R, 2).PadLeft(8, '0')[7];
                lb += cb;
            }
            leght = Convert.ToInt32(lb, 2);
            for (int j = 0; j < (int)(leght * 16 / 3) + 1; j++)
            {
                int rint = random.Next((bitmap.Width - 1) * (bitmap.Height - 1));
                points.Add(new Point(rint/(bitmap.Width-1),rint%(bitmap.Width-1)));
            }
            
            foreach (Point point in points)
            {
                Color color = bitmap.GetPixel(point.X, point.Y);

                string r = Convert.ToString(color.R, 2).PadLeft(8, '0');
                ab.Append(r[7]);
                i += 1;
                if (i % 16 == 0 && i != 0)
                {

                    il += 1;
                    if (il >= leght)
                    {
                        break;
                    }
                    ab.Append(".");
                }
                string g = Convert.ToString(color.G, 2).PadLeft(8, '0');
                ab.Append(g[7]);
                i += 1;
                if (i % 16 == 0 && i != 0)
                {

                    il += 1;
                    if (il >= leght)
                    {
                        break;
                    }
                    ab.Append(".");
                }
                string b = Convert.ToString(color.B, 2).PadLeft(8, '0');
                ab.Append(b[7]);
                i += 1;
                if (i % 16 == 0 && i != 0)
                {

                    il += 1;
                    if (il >= leght)
                    {
                        break;
                    }
                    ab.Append(".");
                }
            }
            string[] vs = ab.ToString().Split('.');
            StringBuilder t = new StringBuilder();
            int li = 0;
            foreach (string c in vs)
            {
                if (li <= leght)
                {
                    int a = Convert.ToInt32(c, 2);
                    char cs = Convert.ToChar(a);
                    t.Append(cs);
                }
                li += 1;
            }
            m_SyncContext.Post(setrichtext, t);
            m_SyncContext.Post(SetTextSafePost, "读取完成");

        }
        private void safewrite()
        {
            Bitmap bitmap = pictureBox1.Image.Clone() as Bitmap;
            char[] c = t.ToCharArray();
            StringBuilder ab = new StringBuilder();
            int i = 0;
            int seed = wkey;
            Random random = new Random(seed);
            List<Point> points = new List<Point>();
            for (int j = 0; j < (int)(t.Length * 16 / 3) + 1; j++)
            {
                int rint = random.Next((bitmap.Width - 1) * (bitmap.Height - 1));
                points.Add(new Point(rint / (bitmap.Width - 1), rint % (bitmap.Width - 1)));
            }
            foreach (char oc in c)
            {
                string by = Convert.ToString(oc, 2).PadLeft(16, '0');
                ab.Append(by);
            }
            
            foreach (Point point in points)
            {
                Color pixel = bitmap.GetPixel(point.X, point.Y);
                string rt, gt, bt;
                int r, g, b;
                r = pixel.R;
                g = pixel.G;
                b = pixel.B;
                rt = Convert.ToString(pixel.R, 2).PadLeft(8, '0');
                gt = Convert.ToString(pixel.G, 2).PadLeft(8, '0');
                bt = Convert.ToString(pixel.B, 2).PadLeft(8, '0');
                if (i < ab.Length)
                {
                    if (rt[7] != ab[i])
                    {
                        if (r < 255)
                        {
                            r += 1;
                        }
                        else
                        {
                            r -= 1;
                        }

                    }
                    i += 1;
                }
                else
                {
                    break;
                }
                if (i < ab.Length)
                {
                    if (gt[7] != ab[i])
                    {
                        if (g < 255)
                        {
                            g += 1;
                        }
                        else
                        {
                            g -= 1;
                        }

                    }
                    i += 1;
                }
                else
                {
                    break;
                }
                if (i < ab.Length)
                {
                    if (bt[7] != ab[i])
                    {
                        if (b < 255)
                        {
                            b += 1;
                        }
                        else
                        {
                            b -= 1;
                        }

                    }
                    i += 1;
                }
                else
                {
                    break;
                }
                bitmap.SetPixel(point.X, point.Y, Color.FromArgb(pixel.A, r, g, b));
            }
            string left = Convert.ToString(le, 2).PadLeft(24, '0');
            for (int ilf = 0; ilf < left.Length; ilf++)
            {

                Color color = bitmap.GetPixel(bitmap.Width - 1, 0);
                string rt = Convert.ToString(color.R, 2).PadLeft(8, '0');
                int r = color.R;
                if (left[ilf] != Convert.ToString(r, 2).PadLeft(8, '0')[7])
                {
                    if (r < 255)
                    {
                        r += 1;
                    }
                    else
                    {
                        r -= 1;
                    }
                }
                bitmap.SetPixel(bitmap.Width - 1, ilf, Color.FromArgb(color.A, r, color.G, color.B));
            }
            m_SyncContext.Post(SetTextSafePost, "写入完成");
            pictureBox2.Image = bitmap.Clone() as Image;
            bitmap.Dispose();
        }
    }
}
