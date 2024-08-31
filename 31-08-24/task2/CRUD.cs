using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using log4net;

namespace Week4AssessmentApp
{
    public class ServerException : Exception
    {
        public ServerException(string message) : base(message) { }
    }

    public class PrescriptionCost
    {
        public int PrescriptionID { get; set; }
        public string PatientName { get; set; }
        public string Medication { get; set; }
        public double Cost { get; set; }

        public override string ToString()
        {
            return $"ID: {PrescriptionID}, Name: {PatientName}, Medication: {Medication}, Cost: ${Cost}";
        }
    }

    public class PrescriptionCostService
    {
        private static string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog = Week4AssessmentDb; Integrated Security = True;";

        public static void Create(PrescriptionCost prescription)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO PrescriptionCosts (PatientName, Medication, Cost) VALUES (@PatientName, @Medication, @Cost)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientName", prescription.PatientName);
                    cmd.Parameters.AddWithValue("@Medication", prescription.Medication);
                    cmd.Parameters.AddWithValue("@Cost", prescription.Cost);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new ServerException($"[0104]Server Error. {ex.Message}");
            }
        }

        public static PrescriptionCost[] Read()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT PrescriptionID, PatientName, Medication, Cost FROM PrescriptionCosts";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    var prescriptions = new List<PrescriptionCost>();
                    while (reader.Read())
                    {
                        prescriptions.Add(new PrescriptionCost
                        {
                            PrescriptionID = (int)reader["PrescriptionID"],
                            PatientName = reader["PatientName"].ToString(),
                            Medication = reader["Medication"].ToString(),
                            Cost = (double)reader["Cost"]
                        });
                    }

                    return prescriptions.ToArray();
                }
            }
            catch (SqlException ex)
            {
                throw new ServerException($"[0102]Server Error. {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new ServerException($"[0103]Server Error. {ex.Message}");
            }
        }

        public static void Update(PrescriptionCost prescription)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE PrescriptionCosts SET PatientName = @PatientName, Medication = @Medication, Cost = @Cost WHERE PrescriptionID = @PrescriptionID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PrescriptionID", prescription.PrescriptionID);
                    cmd.Parameters.AddWithValue("@PatientName", prescription.PatientName);
                    cmd.Parameters.AddWithValue("@Medication", prescription.Medication);
                    cmd.Parameters.AddWithValue("@Cost", prescription.Cost);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new ServerException($"[0105]Server Error. {ex.Message}");
            }
        }

        public static void Delete(int prescriptionID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM PrescriptionCosts WHERE PrescriptionID = @PrescriptionID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PrescriptionID", prescriptionID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new ServerException($"[0106]Server Error. {ex.Message}");
            }
        }
    }

    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("Select an operation:");
                Console.WriteLine("1. Create Prescription");
                Console.WriteLine("2. Read Prescriptions");
                Console.WriteLine("3. Update Prescription");
                Console.WriteLine("4. Delete Prescription");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreatePrescription();
                        break;
                    case "2":
                        ReadPrescriptions();
                        break;
                    case "3":
                        UpdatePrescription();
                        break;
                    case "4":
                        DeletePrescription();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please select a valid option.");
                        break;
                }
            }
        }

        static void CreatePrescription()
        {
            Console.WriteLine("Enter Patient Name:");
            string patientName = Console.ReadLine();
            Console.WriteLine("Enter Medication:");
            string medication = Console.ReadLine();
            Console.WriteLine("Enter Cost:");
            double cost = double.Parse(Console.ReadLine());

            PrescriptionCost prescription = new PrescriptionCost
            {
                PatientName = patientName,
                Medication = medication,
                Cost = cost
            };

            try
            {
                PrescriptionCostService.Create(prescription);
                log.Info("Prescription created successfully.");
            }
            catch (ServerException ex)
            {
                log.Error($"{ex.Message}");
            }
        }

        static void ReadPrescriptions()
        {
            try
            {
                PrescriptionCost[] prescriptions = PrescriptionCostService.Read();
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
                log.Info("Read operation completed.");
            }
            catch (ServerException ex)
            {
                log.Error($"{ex.Message}");
            }
        }

        static void UpdatePrescription()
        {
            Console.WriteLine("Enter Prescription ID to update:");
            int id = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter new Patient Name:");
            string patientName = Console.ReadLine();
            Console.WriteLine("Enter new Medication:");
            string medication = Console.ReadLine();
            Console.WriteLine("Enter new Cost:");
            double cost = double.Parse(Console.ReadLine());

            PrescriptionCost prescription = new PrescriptionCost
            {
                PrescriptionID = id,
                PatientName = patientName,
                Medication = medication,
                Cost = cost
            };

            try
            {
                PrescriptionCostService.Update(prescription);
                log.Info("Prescription updated successfully.");
            }
            catch (ServerException ex)
            {
                log.Error($"{ex.Message}");
            }
        }

        static void DeletePrescription()
        {
            Console.WriteLine("Enter Prescription ID to delete:");
            int id = int.Parse(Console.ReadLine());

            try
            {
                PrescriptionCostService.Delete(id);
                log.Info("Prescription deleted successfully.");
            }
            catch (ServerException ex)
            {
                log.Error($"{ex.Message}");
            }
        }
    }
}
