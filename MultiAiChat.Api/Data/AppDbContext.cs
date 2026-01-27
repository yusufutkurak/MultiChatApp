using Microsoft.EntityFrameworkCore;
using MultiAiChat.Api.Models;
using System.Collections.Generic;

namespace MultiAiChat.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<ChatSession> ChatSession { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }

    }
}