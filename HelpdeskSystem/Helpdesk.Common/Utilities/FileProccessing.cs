using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Helpdesk.Common.Utilities
{
    public class FileProccessing
    {
        /// <summary>
        /// Used to create an empty zip file at the path specified with name received
        /// </summary>
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Used to create an empty zip file at the path specified with name received
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="path">The zip folder location</param>
        /// <returns>The full file file</returns>
        public string CreateZip(string path, string name)
        {
            string fullPath = string.Empty;

            try
            {
                fullPath = $@"{path}{name}.zip";

                var archive = ZipFile.Open(fullPath, ZipArchiveMode.Create);

                archive.Dispose();
            }
            catch (Exception ex)
            {
                fullPath = string.Empty;
                s_logger.Error(ex, "Unable to create zip file.");
            }
            return fullPath;
        }

        /// <summary>
        /// Used to write a datatable as a csv to the zip specified
        /// </summary>
        /// <param name="zipPath">The location of the zip file to write to.</param>
        /// <param name="filename">The name of the CSV being created</param>
        /// <param name="data">The data to put saved in the CSV</param>
        public void SaveToZIPAsCSV(string zipPath, string filename, DataTable data)
        {
            using (FileStream zipToOpen = new FileStream(zipPath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = archive.CreateEntry($"{filename}.csv");
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        WriteDateTableToStream(writer, data);
                    }
                }
            }
        }

        public void WriteDateTableToStream(StreamWriter writer, DataTable data)
        {
            for (int i = 0; i < data.Columns.Count; i++)
            {
                writer.Write(data.Columns[i]);
                if (i < data.Columns.Count - 1)
                {
                    writer.Write(",");
                }
            }

            writer.Write(writer.NewLine);
            foreach (DataRow dr in data.Rows)
            {
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = $"\"{value}\"";
                            writer.Write(value);
                        }
                        else
                        {
                            writer.Write(dr[i].ToString());
                        }
                    }
                    if (i < data.Columns.Count - 1)
                    {
                        writer.Write(",");
                    }
                }
                writer.Write(writer.NewLine);
            }
        }
    }
}
