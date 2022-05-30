using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdAceCodeEditor.Services
{
    public class CloneService
    {
        string LOCALREPO = "LocalRepo";

        public string CloneRepo(GitHubRepo repo)
        {
            string localPath =
                            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(
                            System.Reflection.Assembly.GetEntryAssembly().Location),
                            LOCALREPO, repo.LocalPath, DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));

            Repository.Clone(repo.ClonePath, localPath);

            return localPath;
        }
    }
}
