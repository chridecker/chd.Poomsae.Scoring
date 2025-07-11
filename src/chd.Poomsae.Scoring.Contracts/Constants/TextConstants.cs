﻿using Blazorise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Constants
{
    public class TextConstants
    {
        public const string Accuracy = "Technik";
        public const string Presentation = "Präsentation";
        public const string Total = "Gesamt";
        public const string SpeedAndPower = "Geschwindigkeit und Kraft";
        public const string RhythmAndTempo = "Rhythmus und Tempo";
        public const string ExpressionAndEnergy = "Ausdruck von Energie";

        public const string ChangeColorQuestion = "Farbe wechseln?";
        public const string ResetScoreQuestion = "Wertung zurücksetzen?";

        public const string ExitApplicationQuestion = "Wollen Sie die Anwendung beenden?";
        public const string LeaveSiteQuestion = "Wollen Sie die Seite wirklich verlassen?";
        public const string RemoveFighterQuestion = "Wollen Sie den Teilnehmer wirklich löschen?";
        public const string RemoveRunQuestion = "Wollen Sie den Lauf wirklich löschen?";
        public const string RemoveScoreQuestion = "Wollen Sie die Wertung wirklich löschen?";
        public const string EnterNameTitle = "Anzeigenamen ändern";

        public const string ScoresSend = "Wertung gesendet!";

        public const string Blue = "Blau";
        public const string Red = "Rot";

        public const string Send = "Senden";
        public const string Restart = "Neustart";

        public const string Home = "Home";
        public const string Single = "Einzel";
        public const string Eliminiation = "Head to Head";
        public const string Results = "Ergebnisse";
        public const string Fighters = "Teilnehmer";
        public const string Scores = "Wertung";
        public const string License = "Lizenz";

        public const string SearchFighter = "Teilnehmer Suche";
        public const string AddFighter = "Teilnehmer hinzufügen";
        public const string RemoveFighter = "Teilnehmer löschen";
        public const string EditFighter = "Teilnehmer bearbeiten";
        public const string PrintResult = "Ergebnisliste erstellen";
        
        public const string Form = "Form";
        public const string ChooseForm = "Form Auswahl";

        public const string SavedRun = "Runde erfolgreich gespeichert";

        public const string ResultList = "Ergebnisliste";
        public const string ClassNamePlaceholder = "Klassenname ... ";

        public const string SaveRound = "Speichern";
        public const string Cancel = "Abbrechen";
        public const string AddDeviceAuto = "Geräte automatisches Hinzufügen";
        public const string SearchDevices = "Erweiterte Gerätesuche";
        public const string RemoveDevices = "Geräte Löschen";
        public const string RemoveDevice = "Gerät Entfernen";

        public const string NoLicense = "Es konnte keine gültige Lizenz ermittelt!";
        public const string NoUser = "Es konnte kein Benutzer ermittelt werden!";

        public const string ChooseAll = "Alles auswählen";
        public const string ChooseNone = "Alles abwählen";

        public const string Submit = "Übernehmen";


        private static IEnumerable<string> Taegeuks => Enumerable.Range(1, 8).Select(s => $"Taegeuk {s}");
        private static IEnumerable<string> Dans => ["Koryo", "Keumgang", "Taeback", "Pyongwon", "Shipjin", "Jitae", "Chonkwon", "Hansu", "Ilyo"];

        public static IEnumerable<string> PoomsaeRuns => Taegeuks.Union(Dans);



    }
}
