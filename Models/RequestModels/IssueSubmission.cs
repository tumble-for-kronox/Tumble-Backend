using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.RequestModels;

public class IssueSubmission
{
    public IssueSubmission(string issueTitle, string issueDescription)
    {
        this.title = issueTitle;
        this.description = issueDescription;
    }

    public IssueSubmission() { }

    public string title { get; set; }
    public string description { get; set; }
}
