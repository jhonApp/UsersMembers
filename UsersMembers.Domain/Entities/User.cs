using Amazon.DynamoDBv2.DataModel;
using UsersMembers.Domain.Interfaces;
using System;

namespace UsersMembers.Domain.Entities
{
    [DynamoDBTable("Users")]
    public class User
    {
        // Parameterless constructor for DynamoDB SDK/Serialization
        public User() { }

        // Domain Constructor
        public User(string name, string email, int churchId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
                
            if (!email.Contains("@")) // Simple validation
                throw new ArgumentException("Invalid email format", nameof(email));

            Id = Guid.NewGuid().ToString();
            Name = name;
            Email = email;
            ChurchId = churchId;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            LastSeenAt = DateTime.UtcNow;
        }

        [DynamoDBHashKey]
        public string Id { get; set; }
        public int ChurchId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Profile Profile { get; set; }
        public Address Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeenAt { get; set; }
        public bool IsActive { get; set; }

        public void UpdateName(string newName)
        {
             if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name cannot be empty", nameof(newName));
            Name = newName;
            // UpdatedAt = DateTime.UtcNow; // If we had this field
        }
    }
}
