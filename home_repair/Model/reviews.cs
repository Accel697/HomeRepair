//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace home_repair.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class reviews
    {
        public long idReview { get; set; }
        public long masterReview { get; set; }
        public long clientReview { get; set; }
        public int gradeReview { get; set; }
        public string textReview { get; set; }
    
        public virtual clients clients { get; set; }
        public virtual employees employees { get; set; }
    }
}
