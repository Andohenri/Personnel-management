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
    class ProbationerController
    {
        private static readonly MySqlConnection con = DatabaseController.GetConnection();
        private static int lastInsertIdInfoProbationer;

        internal static void Insert(Probationer probationer)
        {
            Validation(probationer);
            con.Open();
            using MySqlTransaction transaction = con.BeginTransaction();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO info_stagiaire (nom_stagiaire, prenom_stagiaire, contact, niveau, filiere, etablissement)" +
                " VALUES (@nom, @prenom, @contact, @niveau, @filiere, @etab); SELECT LAST_INSERT_ID();", con, transaction))
                {
                    cmd.Parameters.AddWithValue("@nom", probationer.Info_Stagiaire.Nom);
                    cmd.Parameters.AddWithValue("@prenom", probationer.Info_Stagiaire.Prenom);
                    cmd.Parameters.AddWithValue("@contact", probationer.Info_Stagiaire.Contact);
                    cmd.Parameters.AddWithValue("@niveau", probationer.Info_Stagiaire.Niveau);
                    cmd.Parameters.AddWithValue("@filiere", probationer.Info_Stagiaire.Filiere);
                    cmd.Parameters.AddWithValue("@etab", probationer.Info_Stagiaire.Etablissement);
                    lastInsertIdInfoProbationer = Convert.ToInt32(cmd.ExecuteScalar());
                }
                using (MySqlCommand cmd1 = new MySqlCommand("INSERT INTO stagiaire (id_info_stagiaire, id_fichier, id_perso, theme, note, date_debut_stage, duration)" +
                " VALUES (@id_info, @id_fichier, @id_perso, @theme, @note, @date, @duration)", con, transaction))
                {
                    cmd1.Parameters.AddWithValue("@id_info", lastInsertIdInfoProbationer);
                    cmd1.Parameters.AddWithValue("@id_fichier", probationer.IdFile);
                    cmd1.Parameters.AddWithValue("@id_perso", probationer.IdEncadreur);
                    cmd1.Parameters.AddWithValue("@theme", probationer.Theme);
                    cmd1.Parameters.AddWithValue("@note", probationer.Note);
                    cmd1.Parameters.AddWithValue("@date", probationer.DateEnter);
                    cmd1.Parameters.AddWithValue("@duration", probationer.Duration);
                    cmd1.ExecuteNonQuery();
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
        internal static void Update(Probationer probationer, int id)
        {
            Validation(probationer);
            con.Open();
            using MySqlTransaction transaction = con.BeginTransaction();
            try
            {
                int id_info_stagiaire = GetIdInfoProbationner(id);
                using (MySqlCommand cmd = new MySqlCommand("UPDATE info_stagiaire SET nom_stagiaire=@nom, prenom_stagiaire=@prenom, contact=@contact, niveau=@niveau, filiere=@filiere, etablissement=@etab" +
                $" WHERE id_info_stagiaire={id_info_stagiaire}", con, transaction))
                {
                    cmd.Parameters.AddWithValue("@nom", probationer.Info_Stagiaire.Nom);
                    cmd.Parameters.AddWithValue("@prenom", probationer.Info_Stagiaire.Prenom);
                    cmd.Parameters.AddWithValue("@contact", probationer.Info_Stagiaire.Contact);
                    cmd.Parameters.AddWithValue("@niveau", probationer.Info_Stagiaire.Niveau);
                    cmd.Parameters.AddWithValue("@filiere", probationer.Info_Stagiaire.Filiere);
                    cmd.Parameters.AddWithValue("@etab", probationer.Info_Stagiaire.Etablissement);
                    cmd.ExecuteNonQuery();
                }
                using (MySqlCommand cmd1 = new MySqlCommand("UPDATE stagiaire SET id_fichier=@id_fichier, id_perso=@id_perso, theme=@theme, note=@note, date_debut_stage=@date, duration=@duration" +
                $" WHERE id_stagiaire={id}", con, transaction))
                {
                    cmd1.Parameters.AddWithValue("@id_fichier", probationer.IdFile);
                    cmd1.Parameters.AddWithValue("@id_perso", probationer.IdEncadreur);
                    cmd1.Parameters.AddWithValue("@theme", probationer.Theme);
                    cmd1.Parameters.AddWithValue("@note", probationer.Note);
                    cmd1.Parameters.AddWithValue("@date", probationer.DateEnter);
                    cmd1.Parameters.AddWithValue("@duration", probationer.Duration);
                    cmd1.ExecuteNonQuery();
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
        internal static void Delete(int id)
        {
            con.Open();
            using MySqlTransaction transaction = con.BeginTransaction();
            try
            {
                int id_info_stagiaire = GetIdInfoProbationner(id);
                using MySqlCommand cmd1 = new MySqlCommand($"DELETE from stagiaire WHERE id_stagiaire={id}", con, transaction);
                cmd1.ExecuteNonQuery();
                using MySqlCommand cmd = new MySqlCommand($"DELETE FROM info_stagiaire WHERE id_info_stagiaire = {id_info_stagiaire}", con, transaction);
                cmd.ExecuteNonQuery();
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
        private static int GetIdInfoProbationner(int id)
        {
            var query = "SELECT id_info_stagiaire FROM stagiaire WHERE id_stagiaire = @id";
            using MySqlCommand cmd = new MySqlCommand(query, con);
            int result = 0;
            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetInt32("id_info_stagiaire");
            }
            return result;
        }
        private static void Validation(Probationer probationer)
        {
            ValidationContext context = new ValidationContext(probationer);
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(probationer, context, results, true))
            {
                foreach (var error in results)
                {
                    throw new ErrorValidationException(error.ErrorMessage);
                }
            }
        }
    }
}
