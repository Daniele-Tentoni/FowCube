﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FowCube.Utils.Strings {
    using System;
    
    
    /// <summary>
    ///   Classe di risorse fortemente tipizzata per la ricerca di stringhe localizzate e così via.
    /// </summary>
    // Questa classe è stata generata automaticamente dalla classe StronglyTypedResourceBuilder.
    // tramite uno strumento quale ResGen o Visual Studio.
    // Per aggiungere o rimuovere un membro, modificare il file con estensione ResX ed eseguire nuovamente ResGen
    // con l'opzione /str oppure ricompilare il progetto VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AppStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppStrings() {
        }
        
        /// <summary>
        ///   Restituisce l'istanza di ResourceManager nella cache utilizzata da questa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FowCube.Utils.Strings.AppStrings", typeof(AppStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Esegue l'override della proprietà CurrentUICulture del thread corrente per tutte le
        ///   ricerche di risorse eseguite utilizzando questa classe di risorse fortemente tipizzata.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Exception thrown: {0}.
        /// </summary>
        public static string ExceptionMessage {
            get {
                return ResourceManager.GetString("ExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Made with {0} by Daniele Tentoni @2020..
        /// </summary>
        public static string LoginSubTitle {
            get {
                return ResourceManager.GetString("LoginSubTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Collections views.
        /// </summary>
        public static string MenuSubTitleCollection {
            get {
                return ResourceManager.GetString("MenuSubTitleCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Settings views.
        /// </summary>
        public static string MenuSubTitleSettings {
            get {
                return ResourceManager.GetString("MenuSubTitleSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a About.
        /// </summary>
        public static string MenuTitleAbout {
            get {
                return ResourceManager.GetString("MenuTitleAbout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Collections.
        /// </summary>
        public static string MenuTitleCollection {
            get {
                return ResourceManager.GetString("MenuTitleCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Settings.
        /// </summary>
        public static string MenuTitleSettings {
            get {
                return ResourceManager.GetString("MenuTitleSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Sign Out.
        /// </summary>
        public static string MenuTitleSignOut {
            get {
                return ResourceManager.GetString("MenuTitleSignOut", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Add Card to {0}.
        /// </summary>
        public static string PageTitleAddCards {
            get {
                return ResourceManager.GetString("PageTitleAddCards", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Cards.
        /// </summary>
        public static string PageTitleCards {
            get {
                return ResourceManager.GetString("PageTitleCards", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Cards of {0}.
        /// </summary>
        public static string PageTitleCollectionCards {
            get {
                return ResourceManager.GetString("PageTitleCollectionCards", resourceCulture);
            }
        }
    }
}
