﻿using System.ComponentModel.DataAnnotations;

namespace Library.Models;

public class Category
{   
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Book>? Books { get; set; }

    public override string ToString()
    {
        return Name;
    }
}