using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppManagement.models;
using MySql.Data.MySqlClient;
using AppManagement.utils;

namespace AppManagement.controllers
{
    class PersonnelController
    {
        private static readonly MySqlConnection con = DatabaseController.GetConnection();
        private static int lastInsertIdInfo;
        private static int lastInsertIdCin;
        public static void Insert(Personnel personnel)
        {
            Validation(personnel);
            con.Open();
            using (MySqlTransaction transaction = con.BeginTransaction())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO info_perso (nom, prenom, sexe, adresse, contact)" +
                    " VALUES (@nom, @prenoms, @sexe, @adresse, @contact); SELECT LAST_INSERT_ID();", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@nom", personnel.InfoPerso.Nom);
                        cmd.Parameters.AddWithValue("@prenoms", personnel.InfoPerso.Prenoms);
                        cmd.Parameters.AddWithValue("@sexe", personnel.InfoPerso.Sexe);
                        cmd.Parameters.AddWithValue("@adresse", personnel.InfoPerso.Adresse);
                        cmd.Parameters.AddWithValue("@contact", personnel.InfoPerso.Contact);
                        lastInsertIdInfo = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    using (MySqlCommand cmd1 = new MySqlCommand("INSERT INTO cin (num_cin, date_delivrance, lieu_delivrance, duplicata, lieu_duplicata)" +
                    " VALUES (@num_cin, @date_delivrance, @lieu_delivrance, @duplicata, @lieu_duplicata); SELECT LAST_INSERT_ID();", con, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@num_cin", personnel.Cin.Num_cin);
                        cmd1.Parameters.AddWithValue("@date_delivrance", personnel.Cin.Date_delivrance);
                        cmd1.Parameters.AddWithValue("@lieu_delivrance", personnel.Cin.Lieu_delivrance);
                        cmd1.Parameters.AddWithValue("@duplicata", personnel.Cin.Duplicata);
                        cmd1.Parameters.AddWithValue("@lieu_duplicata", personnel.Cin.Lieu_duplicata);
                        lastInsertIdCin = Convert.ToInt32(cmd1.ExecuteScalar());
                    }
                    using (MySqlCommand cmd2 = new MySqlCommand("INSERT INTO personnel (id_info, id_cin, id_attribution, immatricule, date_entree, statut, region)" +
                    " VALUES (@id_info, @id_cin, @id_attribut, @immatricule, @date_entree, @statut, @region)", con, transaction))
                    {
                        cmd2.Parameters.AddWithValue("@id_info", lastInsertIdInfo);
                        cmd2.Parameters.AddWithValue("@id_cin", lastInsertIdCin);
                        cmd2.Parameters.AddWithValue("@id_attribut", personnel.Id_attribut);
                        cmd2.Parameters.AddWithValue("@immatricule", personnel.Immatricule);
                        cmd2.Parameters.AddWithValue("@date_entree", personnel.Date_entree);
                        cmd2.Parameters.AddWithValue("@statut", personnel.Statut);
                        cmd2.Parameters.AddWithValue("@region", personnel.Region);
                        cmd2.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
                finally
                {
                    con.Close();
                }
            }

        }
        public static void Update(Personnel personnel, int id)
        {
            Validation(personnel);
            con.Open();
            using (MySqlTransaction transaction = con.BeginTransaction())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE info_perso SET nom=@nom, prenom=@prenom, sexe=@sexe, adresse=@adresse, contact=@contact WHERE id_info=@id_info", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@nom", personnel.InfoPerso.Nom);
                        cmd.Parameters.AddWithValue("@prenom", personnel.InfoPerso.Prenoms);
                        cmd.Parameters.AddWithValue("@sexe", personnel.InfoPerso.Sexe);
                        cmd.Parameters.AddWithValue("@adresse", personnel.InfoPerso.Adresse);
                        cmd.Parameters.AddWithValue("@contact", personnel.InfoPerso.Contact);
                        cmd.Parameters.AddWithValue("@id_info", id);
                        cmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd1 = new MySqlCommand("UPDATE cin SET num_cin=@num_cin, date_delivrance=@date_delivrance, lieu_delivrance=@lieu_delivrance, duplicata=@duplicata, lieu_duplicata=@lieu_duplicata WHERE id_cin=@id_cin", con, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@num_cin", personnel.Cin.Num_cin);
                        cmd1.Parameters.AddWithValue("@date_delivrance", personnel.Cin.Date_delivrance);
                        cmd1.Parameters.AddWithValue("@lieu_delivrance", personnel.Cin.Lieu_delivrance);
                        cmd1.Parameters.AddWithValue("@duplicata", personnel.Cin.Duplicata);
                        cmd1.Parameters.AddWithValue("@lieu_duplicata", personnel.Cin.Lieu_duplicata);
                        cmd1.Parameters.AddWithValue("@id_cin", id);
                        cmd1.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd2 = new MySqlCommand("UPDATE personnel SET id_attribution=@id_attribution, immatricule=@immatricule, date_entree=@date_entree, statut=@statut, region=@region WHERE id_perso=@id_perso", con, transaction))
                    {
                        cmd2.Parameters.AddWithValue("@id_attribution", personnel.Id_attribut);
                        cmd2.Parameters.AddWithValue("@immatricule", personnel.Immatricule);
                        cmd2.Parameters.AddWithValue("@date_entree", personnel.Date_entree);
                        cmd2.Parameters.AddWithValue("@statut", personnel.Statut);
                        cmd2.Parameters.AddWithValue("@region", personnel.Region);
                        cmd2.Parameters.AddWithValue("@id_perso", id);
                        cmd2.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public static void Delete(int id)
        {
            con.Open();
            using (MySqlTransaction transaction = con.BeginTransaction())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM personnel WHERE id_perso=@id_perso", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id_perso", id);
                        cmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd1 = new MySqlCommand("DELETE FROM info_perso WHERE id_info=@id_info", con, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@id_info", id);
                        cmd1.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd2 = new MySqlCommand("DELETE FROM cin WHERE id_cin=@id_cin", con, transaction))
                    {
                        cmd2.Parameters.AddWithValue("@id_cin", id);
                        cmd2.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public static void Validation(Personnel personnel)
        {
            ValidationContext context = new ValidationContext(personnel, null, null) ;
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(personnel, context, results, true))
            {
                foreach (var error in results)
                {
                    throw new ErrorValidationException(error.ErrorMessage);
                }
            }
        }
    }
}
