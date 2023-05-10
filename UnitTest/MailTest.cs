using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;

namespace Testing;

[TestClass]
public class MailTest : DbTestBase
{
    // [TestMethod]
    // public void Mail_test()
    // {
    //     Email mail = new Email();
    //     mail.EmailAddress = "nataliakoziara6@gmail.com";
    //     mail.Title = "Greenhouse is overheated";
    //     mail.Body = "You stupid";
    //
    //     IEmailDao dao = new EmailEfcDao(DbContext);
    //     IEmailLogic presetLogic = new EmailLogic(dao);
    //     presetLogic.sendMail(mail);
    // }
}