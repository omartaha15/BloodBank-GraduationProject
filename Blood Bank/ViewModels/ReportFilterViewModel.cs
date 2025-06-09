namespace Blood_Bank.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace Blood_Bank.ViewModels
    {
        public class ReportFilterViewModel
        {
            [Display( Name = "Report Period" )]
            public string Period { get; set; } = "monthly";

            [Display( Name = "From" )]
            [DataType( DataType.Date )]
            public DateTime? CustomFrom { get; set; }

            [Display( Name = "To" )]
            [DataType( DataType.Date )]
            public DateTime? CustomTo { get; set; }
        }
    }

}
