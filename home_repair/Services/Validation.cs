using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using home_repair.Model;

namespace home_repair.Services
{
    internal class Validation
    {
        public class ClientValidator
        {
            public (bool isValid, List<string> errors) Validate(clients client)
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(client.lastNameClient) || client.lastNameClient.Length < 2 || client.lastNameClient.Length > 30)
                    errors.Add("Фамилия должна содержать от 2 до 30 символов.");

                if (string.IsNullOrWhiteSpace(client.firstNameClient) || client.firstNameClient.Length < 2 || client.firstNameClient.Length > 30)
                    errors.Add("Имя должно содержать от 2 до 30 символов.");

                if (!string.IsNullOrWhiteSpace(client.middleNameClient) && (client.middleNameClient.Length < 2 || client.middleNameClient.Length > 30))
                    errors.Add("Отчество должно содержать от 2 до 30 символов.");

                if (string.IsNullOrWhiteSpace(client.loginClient) || client.loginClient.Length < 2 || client.loginClient.Length > 50)
                    errors.Add("Логин должен содержать от 2 до 50 символов.");

                if (string.IsNullOrWhiteSpace(client.passwordClient) || client.passwordClient.Length < 8 || client.passwordClient.Length > 200)
                    errors.Add("Пароль должен содержать от 8 до 200 символов.");

                if (string.IsNullOrWhiteSpace(client.emailClient) || client.emailClient.Length > 100)
                    errors.Add("Email должен содержать до 100 символов");

                if (!string.IsNullOrWhiteSpace(client.phoneNumberClient) && client.phoneNumberClient.Length > 12)
                    errors.Add("Номер телефона должен содержать до 12 символов.");

                return (errors.Count == 0, errors);
            }
        }

        public class EmployeeValidator
        {
            public (bool isValid, List<string> errors) Validate(employees employee)
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(employee.lastNameEmployee) || employee.lastNameEmployee.Length < 2 || employee.lastNameEmployee.Length > 30)
                    errors.Add("Фамилия должна содержать от 2 до 30 символов.");

                if (string.IsNullOrWhiteSpace(employee.firstNameEmployee) || employee.firstNameEmployee.Length < 2 || employee.firstNameEmployee.Length > 30)
                    errors.Add("Имя должно содержать от 2 до 30 символов.");

                if (!string.IsNullOrWhiteSpace(employee.middleNameEmployee) && (employee.middleNameEmployee.Length < 2 || employee.middleNameEmployee.Length > 30))
                    errors.Add("Отчество должно содержать от 2 до 30 символов.");

                if (employee.birthDateEmployee == default(DateTime))
                    errors.Add("Дата рождения обязательна.");

                if (employee.genderEmployee <= 0)
                    errors.Add("Пол обязателен.");

                if (employee.positionAtWork <= 0)
                    errors.Add("Должность на работе обязательна.");

                if (employee.wages <= 0)
                    errors.Add("Заработная плата не может быть отрицательной.");

                if (string.IsNullOrWhiteSpace(employee.phoneNumberEmployee) || employee.phoneNumberEmployee.Length > 12)
                    errors.Add("Номер телефона должен содержать до 12 символов.");

                return (errors.Count == 0, errors);
            }
        }

        public class UserValidator
        {
            public (bool isValid, List<string> errors) Validate(users user)
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(user.loginUser) || user.loginUser.Length < 2 || user.loginUser.Length > 50)
                    errors.Add("Логин должен содержать от 2 до 50 символов.");

                if (string.IsNullOrWhiteSpace(user.passwordUser) || user.passwordUser.Length < 8 || user.passwordUser.Length > 200)
                    errors.Add("Пароль должен содержать от 8 до 200 символов.");

                if (user.employeeData <= 0)
                    errors.Add("Данные сотрудника обязательны.");

                return (errors.Count == 0, errors);
            }
        }

        public class VisitValidator
        {
            public (bool isVisited, List<string> errors) Visit(visits visit)
            {
                var errors = new List<string>();

                if (visit.masterVisit < 0)
                    errors.Add("Неверный id мастера.");

                if (visit.clientVisit < 0)
                    errors.Add("Неверный id клмента.");

                if (string.IsNullOrWhiteSpace(visit.phoneNumberVisit) || visit.phoneNumberVisit.Length > 12)
                    errors.Add("Номер телефона должен содержать до 12 символов.");

                if (string.IsNullOrWhiteSpace(visit.adressVisit) || visit.adressVisit.Length > 100)
                    errors.Add("Адрес должен содержать до 100 символов.");

                if (visit.datetimeVisit != null && visit.datetimeVisit == default(DateTime))
                    errors.Add("Дата и время должны быть правильно записаны.");

                if (visit.priceVisit <= 0)
                    errors.Add("Цена вызова не может быть отрицательной.");

                if (!string.IsNullOrWhiteSpace(visit.commentVisit) && visit.commentVisit.Length > 500)
                    errors.Add("Комментарий должен содержать до 500 символов.");

                if (visit.statusVisit <= 0)
                    errors.Add("Статус вызова обязателен.");

                return (errors.Count == 0, errors);
            }
        }

        public class ReviewValidator
        {
            public (bool isValid, List<string> errors) Validate(reviews review)
            {
                var errors = new List<string>();

                if (review.masterReview <= 0)
                    errors.Add("Неверный id мастера.");

                if (review.clientReview <= 0)
                    errors.Add("Неверный id клиента.");

                if (review.gradeReview < 1 || review.gradeReview > 5)
                    errors.Add("Оценка должна быть от 1 до 5.");

                if (!string.IsNullOrWhiteSpace(review.textReview) && review.textReview.Length > 500)
                    errors.Add("Текст отзыва должен содержать до 500 символов.");

                return (errors.Count == 0, errors);
            }
        }
    }
}
