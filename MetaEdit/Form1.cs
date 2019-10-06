using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetaEdit
{
    public partial class frmMetaEdit : Form
    {
        private string[] _mp3Files;
        private int  _counter = 0;
        private TagLib.File _meta;
        private string _folder;

        public frmMetaEdit()
        {
            InitializeComponent();
        }

        private void btnFD_Click(object sender, EventArgs e)
        {
            _counter = 0;
            _folder = "";
            _meta = null;
            _mp3Files = null;

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtFile.Text = fbd.SelectedPath;
                    _mp3Files = Directory.GetFiles(fbd.SelectedPath,"*.mp3");
                    loadFile(0);
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            loadFile(++_counter);
        }

        private void loadFile(int c)
        {
            if (!checkFile(c)) return;
           

            lblFile.Text = Path.GetFileName(_mp3Files[c]);

            _meta = TagLib.File.Create(_mp3Files[c]);
            txtArtist.Text = (_meta.Tag.Performers.Count() > 0) ? _meta.Tag.Performers[0].ToString() : "";
            txtTitle.Text = _meta.Tag.Title;
            txtFileName.Text = lblFile.Text.Replace(".mp3","");

        }

        private bool checkFile(int c)
        {

            if (_mp3Files.Count() > c)
            {
                btnSave.Enabled = true;
                _folder = Path.GetDirectoryName(_mp3Files[c]);
            }
            else
            {
                btnSave.Enabled = false;
                btnNext.Enabled = false;
                lblFile.Text = "Please select the folder.";
                txtArtist.Text = "";
                txtFileName.Text = "";
                txtTitle.Text = "";
                return false;
            }

            if (_mp3Files.Count() > 0) btnNext.Enabled = true;
            if (Path.GetExtension(_mp3Files[c]) != ".mp3")
            {
                _mp3Files[c] = "";
                checkFile(++c);
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _meta.Tag.Performers = new String[1] { txtArtist.Text };
            _meta.Tag.Title = txtTitle.Text;
            _meta.Save();
            string newFile = _folder + "\\" + txtFileName.Text + ".mp3";
            if (!File.Exists(_meta.Name))
            {
                MessageBox.Show("There is no file: \n" + _meta.Name);
                return;
            }

            //if (File.Exists(newFile)) File.Delete(newFile);
            File.Move(_meta.Name, newFile);
            loadFile(++_counter);
        }

        private void frmMetaEdit_DragDrop(object sender, DragEventArgs e)
        {
            _mp3Files = (string[])e.Data.GetData(DataFormats.FileDrop,false);
           
            loadFile(0);
        }

        private void frmMetaEdit_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
