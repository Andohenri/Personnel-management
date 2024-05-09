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
    class DecisionController
    {
        private static readonly MySqlConnection con = DatabaseController.GetConnection();
        private static int prevIdPerso;
        private static int idPerso;

        public static void Insert(Decision decision)
        {
            Validation(decision);
            try
            {
                con.Open();
                idPerso = GetIdPerso(decision.Immatricule);
                using (MySqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        if (!AlreadyHasDecision(idPerso))
                        {
                            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO decision (num_decision, id_perso, nbr_jour_acc, date_decision, id_annee, preuve)" +
                                                                       " VALUES (@num_decision, @id_perso, @nbr_jour_acc, @date_decision, @id_annee, @proof)", con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@num_decision", decision.NumDecision);
                                cmd.Parameters.AddWithValue("@id_perso", idPerso);
                                cmd.Parameters.AddWithValue("@nbr_jour_acc", decision.NbrJourAcc);
                                cmd.Parameters.AddWithValue("@date_decision", decision.DecisionDate);
                                cmd.Parameters.AddWithValue("@id_annee", decision.IdAnnee);
                                cmd.Parameters.AddWithValue("@proof", decision.Proof);
                                cmd.ExecuteNonQuery();

                            }
                            using (MySqlCommand cmd1 = new MySqlCommand($"UPDATE personnel SET nbr_jour_conge=(nbr_jour_conge + @day) WHERE id_perso=@id", con, transaction))
                            {
                                cmd1.Parameters.AddWithValue("@day", decision.NbrJourAcc);
                                cmd1.Parameters.AddWithValue("@id", idPerso);
                                cmd1.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                        else
                        {
                            throw new ErrorValidationException("Ce matricule a déja une décision de congé cette année");
                        }
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show("Erreur Insert:" + e.Message, "Erreur", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (ErrorValidationException ex)
            {
                throw new ErrorValidationException(ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception("Erreur Insert:" + e.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public static void Update(Decision decision, int id, int prevNbrJour, string prevIM)
        {
            Validation(decision);
            try
            {
                con.Open();
                idPerso = GetIdPerso(decision.Immatricule);
                prevIdPerso = GetIdPerso(prevIM);

                using (MySqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        if (idPerso == prevIdPerso)
                        {
                            using (MySqlCommand cmd = new MySqlCommand("UPDATE decision d SET num_decision=@num_decision, id_perso=@id_perso, nbr_jour_acc=@nbr_jour_acc, date_decision=@date_decision, id_annee=@year, preuve=@proof " +
                                                                       "WHERE id_decision = @id_decision", con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@num_decision", decision.NumDecision);
                                cmd.Parameters.AddWithValue("@id_perso", prevIdPerso);
                                cmd.Parameters.AddWithValue("@nbr_jour_acc", decision.NbrJourAcc);
                                cmd.Parameters.AddWithValue("@date_decision", decision.DecisionDate);
                                cmd.Parameters.AddWithValue("@year", decision.IdAnnee);
                                cmd.Parameters.AddWithValue("@proof", decision.Proof);
                                cmd.Parameters.AddWithValue("@id_decision", id);
                                cmd.ExecuteNonQuery();
                            }
                            using (MySqlCommand cmd1 = new MySqlCommand($"UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge - @prevDay) WHERE id_perso = @prevIdPerso", con, transaction))
                            {
                                cmd1.Parameters.AddWithValue("@prevDay", prevNbrJour);
                                cmd1.Parameters.AddWithValue("@prevIdPerso", prevIdPerso);
                                cmd1.ExecuteNonQuery();
                            }
                            using (MySqlCommand cmd2 = new MySqlCommand($"UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge + @day) WHERE id_perso = @id_perso", con, transaction))
                            {
                                cmd2.Parameters.AddWithValue("@day", decision.NbrJourAcc);
                                cmd2.Parameters.AddWithValue("@id_perso", prevIdPerso);
                                cmd2.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            if (!AlreadyHasDecision(idPerso))
                            {
                                using (MySqlCommand cmd = new MySqlCommand("UPDATE decision d SET num_decision=@num_decision, id_perso=@id_perso, nbr_jour_acc=@nbr_jour_acc, date_decision=@date_decision, preuve=@proof " +
                                                                       "WHERE id_decision = @id_decision", con, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@num_decision", decision.NumDecision);
                                    cmd.Parameters.AddWithValue("@id_perso", idPerso);
                                    cmd.Parameters.AddWithValue("@nbr_jour_acc", decision.NbrJourAcc);
                                    cmd.Parameters.AddWithValue("@date_decision", decision.DecisionDate);
                                    cmd.Parameters.AddWithValue("@proof", decision.Proof);
                                    cmd.Parameters.AddWithValue("@id_decision", id);
                                    cmd.ExecuteNonQuery();
                                }
                                using (MySqlCommand cmd1 = new MySqlCommand("UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge - @prevDay) WHERE id_perso = @prevIdPerso", con, transaction))
                                {
                                    cmd1.Parameters.AddWithValue("@prevDay", prevNbrJour);
                                    cmd1.Parameters.AddWithValue("@prevIdPerso", prevIdPerso);
                                    cmd1.ExecuteNonQuery();
                                }
                                using (MySqlCommand cmd2 = new MySqlCommand($"UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge + @day) WHERE id_perso = @id_perso", con, transaction))
                                {
                                    cmd2.Parameters.AddWithValue("@day", decision.NbrJourAcc);
                                    cmd2.Parameters.AddWithValue("@id_perso", idPerso);
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                throw new ErrorValidationException("Ce matricule a déja une décision de congé cette année");
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
            }
            catch (ErrorValidationException ex)
            {
                throw new ErrorValidationException(ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception("Erreur Update:" + e.Message);
            }
            finally
            {
                con.Close();
            }
        }
        internal static bool DownloadProof(int id)
        {
            bool res = false;
            con.Open();
            string query = $"SELECT preuve from decision WHERE id_decision = {id}";
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader r = cmd.ExecuteReader();
            if (r.Read())
            {
                var form = new Form1();

                byte[] content = (byte[])r["preuve"];
                form.saveFileDialog1.Filter = "Fichiers PDF (*.pdf)|*.pdf";
                form.saveFileDialog1.FilterIndex = 1;
                form.saveFileDialog1.Title = "Enregistrer le preuve";
                if (form.saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(form.saveFileDialog1.FileName, content);
                    res = true;
                }
            }
            con.Close();
            return res;
        }
        public static void Delete(int id, int prevNbrJour, int idPerso)
        {
            con.Open();
            using MySqlTransaction transaction = con.BeginTransaction();
            try
            {

                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM decision WHERE id_decision=@id_decision", con, transaction))
                {
                    cmd.Parameters.AddWithValue("@id_decision", id);
                    cmd.ExecuteNonQuery();
                }
                using (MySqlCommand cmd1 = new MySqlCommand("UPDATE personnel SET nbr_jour_conge = (nbr_jour_conge - @prevDay) WHERE id_perso = @prevIdPerso", con, transaction))
                {
                    cmd1.Parameters.AddWithValue("@prevDay", prevNbrJour);
                    cmd1.Parameters.AddWithValue("@prevIdPerso", idPerso);
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
        public static void Validation(Decision decision)
        {
            ValidationContext context = new ValidationContext(decision);
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(decision, context, results, true))
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
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                int result = 0;
                cmd.Parameters.AddWithValue("@immatricule", IM);
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result = reader.GetInt32("id_perso");
                }
                return result;
            }
        }
        private static bool AlreadyHasDecision(int id_perso)
        {
            string current_year = DateTime.Now.Year.ToString();
            using MySqlCommand cmd = new MySqlCommand($"SELECT * FROM decision WHERE id_perso = {id_perso} AND id_annee = {current_year}", con);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool result = reader.HasRows;
            return result;
        }
    }
}
