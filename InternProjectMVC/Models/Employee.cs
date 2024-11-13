using System;
using System.ComponentModel.DataAnnotations;

public class Employee
{
    [Key]
    public int ID { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Birth Date is required.")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(50, ErrorMessage = "Email cannot be longer than 50 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(20, ErrorMessage = "Phonenumber cannot be longer than 10 characters.")]
    [RegularExpression(@"^0[689]\d{8}$", ErrorMessage = "Invalid phone number. Please enter a 10-digit number starting with 0 followed by 6, 8, or 9.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Job Title is required.")]
    [StringLength(50, ErrorMessage = "Job Title cannot be longer than 50 characters.")]
    public string JobTitle { get; set; }
}

