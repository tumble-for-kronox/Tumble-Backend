using KronoxAPI.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.ResponseModels;

public class MultiRegistrationResult
{
    public MultiRegistrationResult(List<AvailableUserEvent> successfulRegistrations, List<AvailableUserEvent> failedRegistrations)
    {
        this.successfulRegistrations = successfulRegistrations;
        this.failedRegistrations = failedRegistrations;
    }

    public List<AvailableUserEvent> successfulRegistrations { get; set; }
    public List<AvailableUserEvent> failedRegistrations { get; set; }


}
