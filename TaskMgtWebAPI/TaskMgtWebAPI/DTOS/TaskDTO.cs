﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskMgtWebAPI.DTOS
{
    public class TaskDTO
    {
        [Key]
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        [ForeignKey("ProjectId")]
        public int ProjectId { get; set; }
    }
}
