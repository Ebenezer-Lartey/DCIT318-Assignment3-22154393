using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // a. Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    // b. Custom exception: invalid score format
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    // c. Custom exception: missing field
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // d. StudentResultProcessor
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var fields = line.Split(',');

                    if (fields.Length < 3)
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: expected 3 fields (ID, Full Name, Score) but found {fields.Length}.");
                    }

                    string idPart = fields[0].Trim();
                    string namePart = fields[1].Trim();
                    string scorePart = fields[2].Trim();

                    if (!int.TryParse(idPart, out int id))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: student ID '{idPart}' is not a valid integer.");
                    }

                    if (!int.TryParse(scorePart, out int score))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: score '{scorePart}' could not be converted to an integer.");
                    }

                    students.Add(new Student(id, namePart, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            string inputFilePath = "students.txt";
            string outputFilePath = "student_report.txt";

            // Create a sample input file for demonstration purposes
            if (!File.Exists(inputFilePath))
            {
                File.WriteAllLines(inputFilePath, new[]
                {
                    "101, Alice Smith, 84",
                    "102, Kwame Boateng, 72",
                    "103, Efua Owusu, 65",
                    "104, John Doe, 45",
                    "105, Mary Johnson, 58"
                });
            }

            var processor = new StudentResultProcessor();

            try
            {
                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);
                processor.WriteReportToFile(students, outputFilePath);
                Console.WriteLine($"Report successfully written to {outputFilePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Input file not found: {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Invalid score format: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Missing field: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
