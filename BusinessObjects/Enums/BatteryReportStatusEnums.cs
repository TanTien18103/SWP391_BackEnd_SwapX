using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum BatteryReportStatusEnums
    {
        Pending, // Waiting for staff action or exchange completion
        Active, // Report is valid and completed
        Inactive, // Report is no longer valid
        Completed, // Exchange and payment are done
        Approved, // Staff has approved the report/form
        Rejected, // Staff has rejected the report/form,
        Cancelled
    }
}