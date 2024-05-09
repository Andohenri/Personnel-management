using AppManagement.models;
using AppManagement.utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppManagement.controllers
{
    class AttributionController
    {
        private static MySqlConnection con = DatabaseController.GetConnection();
        public static void Insert(Attribution attribution)
        {
            var cmd = new MySqlCommand("INSERT INTO attribution (attribut) VALUES (@attribut)", con);
            cmd.Parameters.AddWithValue("@attribut", attribution.Attribut);
            Validation(attribution);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void Update(Attribution attribution)
        {
            var cmd = new MySqlCommand("UPDATE attribution SET attribut=@attribut WHERE id_attribution = @id", con);
            cmd.Parameters.AddWithValue("@id", attribution.Id);
            cmd.Parameters.AddWithValue("@attribut", attribution.Attribut);
            Validation(attribution);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void Delete(Attribution attribut)
        {
            var cmd = new MySqlCommand("DELETE FROM attribution WHERE id_attribution = @id", con);
            cmd.Parameters.AddWithValue("@id", attribut.Id);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new Exception("Erreur de suppression, veuillez ressayer");
            }
            finally
            {
                con.Close();
            }
        }

        private static void Validation(Attribution attr)
        {
            ValidationContext context = new ValidationContext(attr);
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(attr, context, results, true))
            {
                foreach (var error in results)
                {
                    throw new ErrorValidationException(error.ErrorMessage);
                }
            }
        }
    }
}
