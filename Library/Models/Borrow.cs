﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Library.Models;

public class Borrow
{
    public int Id { get; set; }
    public int BookId { get; set; }
    [ForeignKey("BookId")]
    public Book? Book { get; set; }
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }
    public DateTime? BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowStatus Status { get; set; }

    public override string ToString()
    {
        return $"{User.UserName}-{Book.Title}";
    }
}

public enum BorrowStatus
{
    InStock,
    AwaitingPickup,
    Borrowed,
    Returned
}