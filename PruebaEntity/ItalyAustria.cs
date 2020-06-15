using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Core;
using System.Linq;
using System.Threading.Tasks;


namespace PruebaEntity
{
    public class ItalyAustria
    {
        
        private static DateTime thisDay = DateTime.Today;
        private static string formattedDate = thisDay.ToString("dd.MM.yyyy");
        HtmlWeb oWeb = new HtmlWeb();
        HtmlWeb oWebHora = new HtmlWeb();


        public void ItAu()
        {

            //Seleccion de fecha por parte del usuario
            Console.Write("Escriba el dia: ");
            string dia = Console.ReadLine();
            Console.WriteLine("");
            Console.Write("Escriba el mes: ");
            string mes = Console.ReadLine();
            Console.WriteLine("");
            Console.Write("Escriba el año: ");
            string año = Console.ReadLine();
            Console.WriteLine("");
            string punto = ".";

            var fecha = String.Concat(dia,punto,mes,punto,año);

            Console.Write("La fecha introducida es {0} es correcto? s/n: ", fecha);

            //string formattedDate = fecha.ToString("dd.MM.yyyy");
            char afirmacion = Convert.ToChar(Console.ReadLine().ToLower());

            if (afirmacion == 's')
            {
                //Asignacion de valor a la variable URL
                string url = "https://transparency.entsoe.eu/transmission-domain/physicalFlow/show?name=&defaultValue=false&viewType=TABLE&areaType=BORDER_CTY&atch=false&dateTime.dateTime=" + fecha + "+00:00|CET|DAY&border.values=CTY|10YIT-GRTN-----B!CTY_CTY|10YIT-GRTN-----B_CTY_CTY|10YAT-APG------L&dateTime.timezone=CET_CEST&dateTime.timezone_input=CET+(UTC+1)+/+CEST+(UTC+2)";
                string urlHora = "https://transparency.entsoe.eu/transmission-domain/physicalFlow/show?name=&defaultValue=false&viewType=TABLE&areaType=BORDER_CTY&atch=false&dateTime.dateTime=" + fecha + "+00:00|CET|DAY&border.values=CTY|10YIT-GRTN-----B!CTY_CTY|10YIT-GRTN-----B_CTY_CTY|10YAT-APG------L&dateTime.timezone=CET_CEST&dateTime.timezone_input=CET+(UTC+1)+/+CEST+(UTC+2)";



                //Comando de apertura de la libreria HtmlAgilityPack
                HtmlDocument doc = oWeb.Load(url);
                HtmlDocument docHora = oWebHora.Load(urlHora);
                
                //Declaracion de la Lista para guardar los valores leidos de la tabla
                List<string> valoresItAu = new List<string>();
                List<string> valoresItAuPARES = new List<string>(48);
                List<string> valoresItAuIMPARES = new List<string>(48);

                //Declaracion de la Lista para guardar las horas leidas de la tabla
                List<string> valoresHora = new List<string>();





                //Inicializador de contador para indicar par e impar, ayuda a colocar valores en columnas 1(par) y columnas 2(impar)
                int count = 0;
                int counthora = 0;




                //Inicio del ciclo foreach que guardara los valores en la lista, utilizando HtmlAgilityPack
                foreach (var Nodo in doc.DocumentNode.CssSelect(".data-view-detail-link"))
                {
                    valoresItAu.Add(Nodo.InnerHtml);

                    //Se añade el contador para tener el tamaño de la lista
                    count++;
                    if (count % 2 == 0)
                    {
                        valoresItAuIMPARES.Add(Nodo.InnerHtml);

                    }
                    else
                    {
                        valoresItAuPARES.Add(Nodo.InnerHtml);
                    }
                }

                foreach (var NodoHora in docHora.DocumentNode.CssSelect(".first "))
                {
                    valoresHora.Add(NodoHora.InnerHtml);

                    counthora++;
                }




                //inicializacion del EntityFramework con la base de datos (MiEntity = Entidad, db = nombre variable para conexion BBDD)
                using (MiEntity2 db = new MiEntity2())
                {
                    //se inician las variables que voy a insertar en la base de datos
                    tabla oHora = new tabla();
                    tabla oATmayorIT = new tabla();
                    tabla oITmayorAT = new tabla();

             
                    //Ciclo para insertar la hora en la tabla de bbdd

                    for (int i = 0; i < valoresHora.Count; i++)
                    {
                        Console.Write(valoresHora[i] + " ");
                        oHora.Hora = valoresHora[i];
                        db.tabla.Add(oHora);
                        db.SaveChanges();

                        //ciclo para insertar los valores de la columna 1 en la base de datos donde la hora coincida con el primer registrro 
                        for (int x = i; x == i; x++)
                        {
                            oATmayorIT = db.tabla.Where(d => d.Hora == oHora.Hora).First();
                            oATmayorIT.ATmayorIT = valoresItAuPARES[i];
                            db.tabla.Add(oATmayorIT);
                            db.Entry(oATmayorIT).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();


                            //ciclo para insertar los valores de la columna 2 en la base de datos donde la hora coincida con el primer registrro
                            for (int j = i; j == i; j++)
                            {
                                oITmayorAT = db.tabla.Where(d => d.ATmayorIT == oATmayorIT.ATmayorIT).First();
                                oITmayorAT.ITmayorAT = valoresItAuIMPARES[i];
                                db.tabla.Add(oITmayorAT);
                                db.Entry(oITmayorAT).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                            }
                        }
                    }



                }
            }
            else
            {
                Console.WriteLine("Muchas Gracias por usar el programa");
            }

        
        }
    }
}



