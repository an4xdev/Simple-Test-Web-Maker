using System.ComponentModel.DataAnnotations;

namespace Projekt_2.Models;

public class TestQuestion : Question
{
    public List<TestAnswer> TestAnswers { get; set; } = [];
}