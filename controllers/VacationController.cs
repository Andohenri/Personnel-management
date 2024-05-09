using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppManagement.models;
using AppManagement.utils;
using MySql.Data.MySqlClient;

namespace AppManagement.controllers
{
    class VacationController
    {
        private static readonly MySqlConnection con = DatabaseController.GetConnection();
        private static int prevIdPerso;
        private static int idPerso;

        internal static void Insert(Vacation vacation)
        {
            con.Open();
            idPerso = GetIdPerso(vacation.Immatricule);
            Validation(vacation);

            using MySqlTransaction transaction = con.BeginTransaction();
            try
            {
                if (vacation.NbrJour <= VacationDispo(idPerso))
                {
                    using (MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO conge (id_perso, nbr_jour, date_debut, date_fin, motif, remplacent, lieu_conge)" +
                    " VALUES (@id_perso, @nbr_jour, @date_debut, @date_fin, @motif, @interime, @lieu_conge)", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id_perso", idPerso);
                        cmd.Parameters.AddWithValue("@date_debut", vacation.DateDebut);
                        cmd.Parameters.AddWithValue("@nbr_jour", vacation.NbrJour);
                        cmd.Parameters.AddWithValue("@date_fin", vacation.DateFin);
                        cmd.Parameters.AddWithValue("@motif", vacation.Motif);
                        cmd.Parameters.AddWithValue("@interime", vacation.Substitute);
                        cmd.Parameters.AddWithValue("@lieu_conge", vacation.Lieu);
                        cmd.ExecuteNonQuery();

                    }
                    if(vacation.Motif == "Congés Annuels")
                    {
                        using MySqlCommand cmd1 = new MySqlCommand($"UPDATE personnel SET nbr_jour_conge=(nbr_jour_conge - @day) WHERE id_perso=@id", con, transaction);
                        cmd1.Parameters.AddWithValue("@day", vacation.NbrJour);
                        cmd1.Parameters.AddWithValue("@id", idPerso);
                        cmd1.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                else
                {
                    throw new ErrorValidationException($"Nombre de jour de congé insuffisant ({VacationDispo(idPerso)} restant)");
                }
            }
            catch (MySqlException e)
            {
                transaction.Rollback();
                throw new Exception("Erreur Insert:" + e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        internal static void Update(Vacation vacation, int id, string prevIM, int prevNbrJour)
        {
            con.Open();
            idPerso = GetIdPerso(vacation.Immatricule);
            prevIdPerso = GetIdPerso(prevIM);
            Validation(vacation);
            using MySqlTransaction transaction = con.BeginTransaction();
            try
            {
                if (idPerso == prevIdPerso)
                {
                    using (MySqlCommand cmd1 = new MySqlCommand($"UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge + @prevDay) WHERE id_perso = @prevIdPerso", con, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@prevDay", prevNbrJour);
                        cmd1.Parameters.AddWithValue("@prevIdPerso", prevIdPerso);
                        cmd1.ExecuteNonQuery();
                    }
                    if (vacation.NbrJour <= VacationDispo(prevIdPerso) )
                    {
                        using (MySqlCommand cmd = new MySqlCommand("UPDATE conge SET id_perso=@id_perso, nbr_jour=@nbr_jour, " +
                                                                    "date_debut=@date_debut, date_fin=@date_fin, motif=@motif, remplacent=@interime, lieu_conge=@lieu_conge" +
                                                                   " WHERE id_conge=@id_conge", con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id_perso", prevIdPerso);
                            cmd.Parameters.AddWithValue("@date_debut", vacation.DateDebut);
                            cmd.Parameters.AddWithValue("@nbr_jour", vacation.NbrJour);
                            cmd.Parameters.AddWithValue("@date_fin", vacation.DateFin);
                            cmd.Parameters.AddWithValue("@motif", vacation.Motif);
                            cmd.Parameters.AddWithValue("@interime", vacation.Substitute);
                            cmd.Parameters.AddWithValue("@lieu_conge", vacation.Lieu);
                            cmd.Parameters.AddWithValue("@id_conge", id);
                            cmd.ExecuteNonQuery();
                        }
                        if (vacation.Motif == "Congés Annuels")
                        {
                            using MySqlCommand cmd2 = new MySqlCommand($"UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge - @day) WHERE id_perso = @id_perso", con, transaction);
                            cmd2.Parameters.AddWithValue("@day", vacation.NbrJour);
                            cmd2.Parameters.AddWithValue("@id_perso", prevIdPerso);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        int jourDispo = VacationDispo(prevIdPerso);
                        transaction.Rollback();
                        throw new ErrorValidationException($"Cet matricule a {jourDispo} jours disponibles");
                    }

                }
                else
                {
                    using (MySqlCommand cmd1 = new MySqlCommand("UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge + @prevDay) WHERE id_perso = @prevIdPerso", con, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@prevDay", prevNbrJour);
                        cmd1.Parameters.AddWithValue("@prevIdPerso", prevIdPerso);
                        cmd1.ExecuteNonQuery();
                    }
                    int nbr = VacationDispo(idPerso);
                    if (vacation.NbrJour <= nbr)
                    {
                        using (MySqlCommand cmd = new MySqlCommand("UPDATE conge c SET id_perso=@id_perso, nbr_jour=@nbr_jour, date_debut=@date_debut, date_fin=@date_fin, motif=@motif, remplacent=@interime, lieu_conge=@lieu_conge" +
                                                                   " WHERE id_conge=@id_conge", con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id_perso", idPerso);
                            cmd.Parameters.AddWithValue("@date_debut", vacation.DateDebut);
                            cmd.Parameters.AddWithValue("@nbr_jour", vacation.NbrJour);
                            cmd.Parameters.AddWithValue("@date_fin", vacation.DateFin);
                            cmd.Parameters.AddWithValue("@motif", vacation.Motif);
                            cmd.Parameters.AddWithValue("@interime", vacation.Substitute);
                            cmd.Parameters.AddWithValue("@lieu_conge", vacation.Lieu);
                            cmd.Parameters.AddWithValue("@id_conge", id);
                            cmd.ExecuteNonQuery();
                        }
                        if (vacation.Motif == "Congés Annuels")
                        {
                            using MySqlCommand cmd2 = new MySqlCommand($"UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge - @day) WHERE id_perso = @id_perso", con, transaction);
                            cmd2.Parameters.AddWithValue("@day", vacation.NbrJour);
                            cmd2.Parameters.AddWithValue("@id_perso", idPerso);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        throw new ErrorValidationException($"Cet matricule a {VacationDispo(idPerso)} jours disponibles");
                    }
                }
                transaction.Commit();
            }
            catch (MySqlException e)
            {
                transaction.Rollback();
                throw new Exception("Erreur Update:" + e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        internal static void Delete(int id, int prevNbrJour, int idPersoOnDelete, string motif)
        {
            con.Open();
            using MySqlTransaction transaction = con.BeginTransaction();
            try
            {

                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM conge WHERE id_conge=@id_conge", con, transaction))
                {
                    cmd.Parameters.AddWithValue("@id_conge", id);
                    cmd.ExecuteNonQuery();
                }
                if (motif == "Congés Annuels") 
                {
                    using MySqlCommand cmd1 = new MySqlCommand("UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge + @prevDay) WHERE id_perso = @prevIdPerso", con, transaction);
                    cmd1.Parameters.AddWithValue("@prevDay", prevNbrJour);
                    cmd1.Parameters.AddWithValue("@prevIdPerso", idPersoOnDelete);
                    cmd1.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (MySqlException e)
            {
                transaction.Rollback();
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private static void Validation(Vacation vacation)
        {
            ValidationContext context = new ValidationContext(vacation);
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(vacation, context, results, true))
            {
                foreach (var error in results)
                {
                    throw new ErrorValidationException(error.ErrorMessage);
                }
            }
        }

        private static int GetIdPerso(string IM)
        {
            var query = "SELECT id_perso FROM personnel WHERE immatricule = @immatricule";
            using MySqlCommand cmd = new MySqlCommand(query, con);
            int result = 0;
            cmd.Parameters.AddWithValue("@immatricule", IM);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetInt32("id_perso");
            }
            return result;
        }

        private static int VacationDispo(int idPerso)
        {
            var query = "SELECT nbr_jour_conge FROM personnel WHERE id_perso = @id_perso";
            using MySqlCommand cmd = new MySqlCommand(query, con);
            int result = 0;
            cmd.Parameters.AddWithValue("@id_perso", idPerso);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetInt32("nbr_jour_conge");
            }
            return result;
        }

        internal static void UpdateStatut(string v, int id)
        {
            con.Open();
            var query = $"UPDATE conge SET etat_conge = '{v}' WHERE id_conge = {id}";
            using MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
