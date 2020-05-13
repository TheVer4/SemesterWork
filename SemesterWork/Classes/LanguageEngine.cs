using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SemesterWork
{
    public class LanguageEngine
    {
        private static Dictionary<string, XmlDocument> _languages = new Dictionary<string, XmlDocument>();
        public static List<string> Languages => _languages.Keys.ToList();
        public static string Current { get; set; } = "English";

        public LanguageEngine()
        {
            XmlDocument defaultTranslation = new XmlDocument();
            defaultTranslation.LoadXml(DEFAULT_LANGUAGE);
            _languages["English"] = defaultTranslation;
            foreach (var file in Directory.GetFiles("Languages").Where(x => x.EndsWith(".xml")))
            {
                XmlDocument document = new XmlDocument();
                document.Load(file);
                XmlElement root = document.DocumentElement;
                if(root?.HasChildNodes != true) continue;
                string langName = root?["language"]?.InnerText;
                if(langName == null || langName == "English") continue;
                _languages[langName] = document;
            }
        }

        public string this[string index] => 
            TryGetTranslation(Current, index) ?? TryGetTranslation("English", index);

        private string TryGetTranslation(string language, string index)
        {
            return _languages[language]
                .DocumentElement?["strings"]
                ?.ChildNodes
                .Cast<XmlNode>()
                .FirstOrDefault(x => x.Attributes["name"]?.InnerText.ToString() == index)
                ?.InnerText;
        }

        #region defaultLanguage

        private readonly string DEFAULT_LANGUAGE =
            @"<?xml version='1.0' encoding='utf-8'?>
<document>
    <language>English</language>
    <strings>
        <string name='LoginActivity Account'>Login</string>
        <string name='LoginActivity Password'>Password</string>
        <string name='LoginActivity Authorize'>Authorize</string>
        <string name='LoginActivity Exit'>Exit</string>
        <string name='LoginActivity ExitMessageBoxTitle'>Confirm action</string>
        <string name='LoginActivity ExitMessageBox'>Do you really want to exit?</string>
        <string name='LoginActivity AuthorizationMessageBoxTitle'>An error has occurred</string>
        <string name='LoginActivity AuthorizationMessageBox'>Incorrect login or password, try again.</string>
        <string name='MainMenuActivity FastInvoice'>Fast invoice</string>
        <string name='MainMenuActivity WareHouse'>Warehouse</string>
        <string name='MainMenuActivity Settings'>Settings</string>
        <string name='MainMenuActivity Logout'>Logout</string>
        <string name='FastInvoiceActivity AddPosition'>Add</string>
        <string name='FastInvoiceActivity Cashier'>Cashier</string>
        <string name='FastInvoiceActivity Payment'>PAYMENT</string>
        <string name='FastInvoiceActivity Amount'>AMOUNT</string>
        <string name='FastInvoiceActivity Total'>TOTAL</string>
        <string name='FastInvoiceActivity Position'>Position</string>
        <string name='FastInvoiceActivity Price'>Price</string>
        <string name='FastInvoiceActivity Count'>Amount</string>
        <string name='FastInvoiceActivity Units'>Units</string>
        <string name='FastInvoiceActivity FullPrice'>Full price</string>
        <string name='FastInvoiceActivity AddPositionErrorMessageBoxTitle'>An error has occurred</string>
        <string name='FastInvoiceActivity AddPositionErrorMessageBox'>Position with code {0} not found, try the operation again.</string>
        <string name='WareHouseActivity AddPosition'>Add</string>
        <string name='WareHouseActivity Manager'>Manager</string>
        <string name='WareHouseActivity Save'>Save</string>
        <string name='WareHouseActivity EAN13'>EAN13</string>
        <string name='WareHouseActivity FullName'>Full name</string>
        <string name='WareHouseActivity Price'>Price</string>
        <string name='WareHouseActivity Amount'>Amount</string>
        <string name='WareHouseActivity Units'>Units</string>
        <string name='WareHouseActivity ShortName'>Short name</string>
        <string name='WareHouseActivity EAN13FormatErrorTitle'>An error has occurred</string>
        <string name='WareHouseActivity EAN13FormatError'>Invalid input format EAN13</string>
        <string name='WareHouseActivity ContainsQuestionTitle'>Confirm action</string>
        <string name='WareHouseActivity ContainsQuestion'>Product with code {0} is already in warehouse. Do you want to update the value?</string>
        <string name='WareHouseActivity SingleDeleteConfirmTitle'>Confirm action</string>
        <string name='WareHouseActivity SingleDeleteConfirm'>Are you sure you want to delete this position?</string>
        <string name='WareHouseActivity DeleteConfirmTitle'>Confirm action</string>
        <string name='WareHouseActivity DeleteConfirm'>Are you sure you want to delete all positions?</string>
        <string name='WareHouseActivity SaveMessageBoxTitle'>Message</string>
        <string name='WareHouseActivity SaveMessageBox'>Positions saved successfully!</string>
        <string name='WareHouseActivity PositionContainsQuestionTitle'>Error</string>
        <string name='WareHouseActivity PositionContainsQuestion'>Already have a line item with code {0}.</string>
        <string name='SettingsActivity Language'>Language</string>
        <string name='SettingsActivity NetPrinterName'>Network printer name</string>
        <string name='SettingsActivity NetPrinterAddress'>Full address</string>
        <string name='SettingsActivity ScannerPort'>Scanner COM-Port</string>
        <string name='SettingsActivity Apply'>Apply</string>
        <string name='SettingsActivity Cancel'>Exit</string>
        <string name='SettingsActivity CancelConfirmTitle'>Confirm action</string>
        <string name='SettingsActivity CancelConfirm'>Are you sure you want to exit the settings?</string>
        <string name='BarcodeReader NotFoundExceptionTitle'>Attention!</string>
        <string name='BarcodeReader NotFoundException'>Could not find the scanner. Check the connection and restart the mode from the main menu.</string>
        <string name='Printer NotFoundExceptionTitle'>Attention!</string>
        <string name='Printer NotFoundException'>Could not find the printer. Check the connection and restart the mode from the main menu.</string>
        <string name='WareHouseActivity DeleteFromDBButton'>Delete from warehouse</string>
        <string name='WareHouseActivity DeleteFromDB DeletingErrorTitle'>Error</string>
        <string name='WareHouseActivity DeleteFromDB DeletingError'>Nothing to delete</string>
        <string name='WareHouseActivity DeleteFromDB DeletingPositionsTitle'>Confirm action</string>
        <string name='WareHouseActivity DeleteFromDB DeletingPositions'>Are you sure you want to remove these items from the warehouse?</string>
        <string name='WareHouseActivity DeleteFromDB DeletingPositionTitle'>Confirm action</string>
        <string name='WareHouseActivity DeleteFromDB DeletingPosition'>Are you sure you want to remove this item from the warehouse?</string>
        <string name='WareHouseActivity DeleteFromDB DeletingNotInDBTitle'>Attention!</string>
        <string name='WareHouseActivity DeleteFromDB DeletingNotInDB'>This item is not in the warehouse.</string>
    </strings>
</document>
";

        #endregion
    }
}
