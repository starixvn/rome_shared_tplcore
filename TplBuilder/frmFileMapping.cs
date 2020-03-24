using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;

namespace TplBuilder
{
    public partial class frmFileMapping : Form
    {
        private Execute _execute;

        public frmFileMapping()
        {
            InitializeComponent();
        }

        public frmFileMapping(Execute execute)
        {
            InitializeComponent();

            _execute = execute;
            this.Text = this.Text + " - " + _execute.CurrentProject.Name;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Process();
        }

        private void Process()
        {
            int linkTotal = 0;
            int badLinkTotal = 0;
            try
            {
                var htmItems = new List<ProjectItem>();
                var csItems = new List<ProjectItem>();
                GetValidItems(_execute.CurrentProject.ProjectItems, ref htmItems, ref csItems);

                linkTotal = htmItems.Count;
                foreach (ProjectItem htmItem in htmItems)
                {
                    string src = htmItem.get_FileNames(0);
                    string desc = _execute.GetCsFilePath(htmItem);
                    ProjectItem csLinkItem = null;
                    foreach (ProjectItem csItem in csItems)
                    {
                        string filePath = csItem.get_FileNames(0);
                        if (string.Compare(desc, filePath, true) == 0)
                        {
                            csLinkItem = csItem;
                            break;
                        }
                    }

                    Image img = TplBuilder.Properties.Resources.link;
                    MappingErrorType errorType = MappingErrorType.None;
                    if (csLinkItem == null)
                    {
                        img = TplBuilder.Properties.Resources.link_break;
                        errorType = MappingErrorType.MissingDestination;
                        badLinkTotal++;
                    }
                    else
                    {
                        csItems.Remove(csLinkItem);
                    }

                    src = src.Replace(_execute.ProjectPath, "");
                    desc = desc.Replace(_execute.ProjectPath, "");
                    int rowId = grid.Rows.Add(img, src, desc, errorType);
                    if (csLinkItem == null)
                    {
                        var dgvRow = grid.Rows[rowId];
                        dgvRow.Cells[colDestination.Index].Style.BackColor = Config.NotFoundColor;
                    }
                }

                linkTotal += csItems.Count;
                badLinkTotal += csItems.Count;
                foreach (ProjectItem csItem in csItems)
                {
                    string desc = csItem.get_FileNames(0);
                    string src = _execute.GetHtmFilePath(csItem);
                    src = src.Replace(_execute.ProjectPath, "");
                    desc = desc.Replace(_execute.ProjectPath, "");
                    int rowId = grid.Rows.Add(TplBuilder.Properties.Resources.link_break, src, desc, MappingErrorType.MissingSource);
                    var dgvRow = grid.Rows[rowId];
                    dgvRow.Cells[colSource.Index].Style.BackColor = Config.NotFoundColor;
                }

                lblLinkTotal.Text = string.Format(lblLinkTotal.Text, linkTotal);
                lblBadLinkTotal.Text = string.Format(lblBadLinkTotal.Text, badLinkTotal);
            }
            catch (Exception ex)
            {
                InfoLog.Log(ex);
            }
        }

        private void GetValidItems(ProjectItems items, ref List<ProjectItem> htmItems, ref List<ProjectItem> csItems)
        {
            foreach (ProjectItem item in items)
            {
                if (Utils.IsFile(item))
                {
                    if (Utils.IsHtmExt(item.Name))
                    {
                        htmItems.Add(item);
                    }
                    else if (Utils.IsTplCsExt(item.Name))
                    {
                        string filePath = item.get_FileNames(0);
                        if (Utils.IsTplCsContent(filePath))
                        {
                            csItems.Add(item);
                        }
                    };
                }
                else if (Utils.IsFolder(item))
                {
                    GetValidItems(item.ProjectItems, ref htmItems, ref csItems);
                }
            }
        }

        private void chbOnlyShowBadLinks_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgvRow in grid.Rows)
            {
                if (chbOnlyShowBadLinks.Checked)
                {
                    MappingErrorType errorType = (MappingErrorType)dgvRow.Cells[colMappingErrorType.Index].Value;
                    if (errorType == MappingErrorType.None)
                    {
                        dgvRow.Visible = false;
                    }
                }
                else
                {
                    dgvRow.Visible = true;
                }
            }
        }

        private void chbShowFullPath_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgvRow in grid.Rows)
            {
                if (chbShowFullPath.Checked)
                {
                    dgvRow.Cells[colSource.Index].Value = _execute.ProjectPath + dgvRow.Cells[colSource.Index].Value.ToString();
                    dgvRow.Cells[colDestination.Index].Value = _execute.ProjectPath + dgvRow.Cells[colDestination.Index].Value.ToString();
                }
                else
                {
                    dgvRow.Cells[colSource.Index].Value =  dgvRow.Cells[colSource.Index].Value.ToString().Replace(_execute.ProjectPath, "");
                    dgvRow.Cells[colDestination.Index].Value = dgvRow.Cells[colDestination.Index].Value.ToString().Replace(_execute.ProjectPath, "");
                }
            }
        }
    }
}
