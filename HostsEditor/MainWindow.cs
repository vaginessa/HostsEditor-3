﻿using HostsEditor.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HostsEditor
{
    public partial class MainWindow : Form
    {
        public List<HostRow> Hosts { get; set; }
        private DataManager dataManager;
        private Updater updater;

        public MainWindow()
        {
            InitializeComponent();

            dataManager = new DataManager();
            updater = new Updater();

            PrepareView();
        }

        //       METHODS        //

        /// <summary>
        /// Prepares content in the main window view by loading hosts and providing the total hosts loaded.
        /// </summary>
        private void PrepareView()
        {
            dataManager.LoadHostsFromFile();

            Hosts = dataManager.Hosts;
            CountLabel.Text = "Host entries loaded: " + Hosts.Count.ToString();

            PopulateGrid();
        }

        /// <summary>
        /// Populates the hosts grid with the loaded hosts from the file.
        /// </summary>
        private void PopulateGrid()
        {
            foreach (HostRow hostRow in Hosts)
            {
                string[] entry = {hostRow.IP, hostRow.Host};

                HostsGrid.Rows.Add(entry);
            }

            HostsGrid.AutoResizeColumns();
        }

        /// <summary>
        /// Reloads the contents of the hosts grid.
        /// </summary>
        private void ReloadGrid()
        {
            HostsGrid.Rows.Clear();
            HostsGrid.Refresh();

            PrepareView();
        }

        // INTERACTIVE CONTROLS //

        private void EditButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", Environment.SystemDirectory + @"\drivers\etc\hosts");
        }

        private void ReloadButton_Click(object sender, EventArgs e)
        {
            ReloadGrid();
        }

        private void CLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/aerouk/HostsEditor");
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HostsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView senderGrid = (DataGridView)sender;
            DataGridViewCell cell = senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            EditHostWindow editHostWindow = new EditHostWindow(dataManager, cell.RowIndex, Hosts[cell.RowIndex]);

            editHostWindow.ShowDialog(this);
            ReloadGrid();
        }

        private void NewHostButton_Click(object sender, EventArgs e)
        {
            NewHostWindow newHostWindow = new NewHostWindow(dataManager);

            newHostWindow.ShowDialog(this);
            ReloadGrid(); // reload grid after dialog is closed
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult exitDialogResult = MessageBox.Show("Are you sure you would like to exit?", "Exit application", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (exitDialogResult == DialogResult.No)
                e.Cancel = true;
        }

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!updater.IsCurrent)
                updater.DisplayUpdateLink();
            else
                MessageBox.Show("No update available. You are using the most recent version!");
        }

        // ################### //
    }
}
