using MHS_FINAL_PROJECT.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Data;

namespace MHS_FINAL_PROJECT.Controllers
{


    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session.Abandon();
            return View();
        }


        public ActionResult Normal_search(drug_active_name_serach model)
        {
            Session.Abandon();
            ViewBag.serchFor = model.name;
            if (model.name != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //drug-data
                    //Each Single Data for The Drug Will Be Returned Here In Var data
                    SqlParameter[] par_active = new SqlParameter[] {
                    new SqlParameter("@name" , model.name)
                    };
                    var data = db.Database.SqlQuery<drug_serach_result>("normal_search @name", par_active).ToList();



                    List<side_effect> effect = new List<side_effect>();
                    List<drug_trade_search_name> actives = new List<drug_trade_search_name>();
                    //side_effect
                    //Each Side Effect For The Drug Will Be Returned Here In Var effect
                    SqlParameter[] par_effect = new SqlParameter[] {
                        new SqlParameter("@name" ,  model.name)
                    };
                    effect = db.Database.SqlQuery<side_effect>("side_effects @name", par_effect).ToList();

                    //find If The Enterd Name is a Trade Name Or Not ... If Data Retured In The actives Var Then It Is A Trade Name
                    SqlParameter[] par_actives = new SqlParameter[] {
                            new SqlParameter("@name" , model.name)
                            };
                    actives = db.Database.SqlQuery<drug_trade_search_name>("get_active_for_trade @name", par_actives).ToList();


                    var result = new drug_serach_result_finsh()
                    {
                        active_name_for_trade = actives,
                        drug = data,
                        side_effect = effect,
                    };

                    return View(result);

                }

            }
            return View();
        }





        public ActionResult LoadData(drug_active_name_serach model)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                ApplicationDbContext db = new ApplicationDbContext();
                SqlParameter[] par_trade = new SqlParameter[] {
                    new SqlParameter("@name" , model.name)
                    };
                var trade = db.Database.SqlQuery<drug_trade_name>("trad_names @name", par_trade).ToList();

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    trade = trade.Where(m => (m.trade_name.Contains(searchValue)) || (m.Manufacturer_company.Contains(searchValue))).ToList();
                }
                //total number of rows count     
                recordsTotal = trade.Count();
                //Paging     
                var data = trade.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }






        public ActionResult advanced_search(drug_active_advanced model)
        {
            Session.Abandon();
            ViewBag.trade_name = null;
            ViewBag.serchFor = "no result yet";
            ViewBag.serchForName = model.search_name;
            drug_active_advanced result = new drug_active_advanced();
            if (model.search_name != null)
            {
                ViewBag.select = "select a choices";
                if (!(model.description == false && model.drug_dosage == false && model.warnings == false && model.side_effects == false && model.side_effects == false && model.trade_names == false))
                {
                    //Session["name"] = model.search_name; 
                    ViewBag.select = null;
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        List<drug_active_name> data_desc = null;
                        List<drug_dosage> data_dosage = null;
                        List<warning> data_warning = null;
                        List<side_effect> side_effect = null;
                        List<drug_trade_search_name> actives = null;
                        ViewBag.if_exist = db.drug_active_names.Where(m => m.name == model.search_name).FirstOrDefault();


                        if (model.description == true)
                        {
                            SqlParameter[] par_active = new SqlParameter[] {
                            new SqlParameter("@P_drug_name" , model.search_name),
                            new SqlParameter("@P_drug_id" , -1)
                            };
                            data_desc = db.Database.SqlQuery<drug_active_name>("GET_drug_info @P_drug_name , @P_drug_id", par_active).ToList();
                        }

                        if (model.drug_dosage == true)
                        {
                            SqlParameter[] par_active = new SqlParameter[] {
                                new SqlParameter("@P_drug_name" , model.search_name),
                                new SqlParameter("@P_drug_id" , -1)
                            };
                            data_dosage = db.Database.SqlQuery<drug_dosage>("GET_drug_dosage @P_drug_name, @P_drug_id", par_active).ToList();
                        }

                        if (model.warnings == true)
                        {
                            SqlParameter[] par_active = new SqlParameter[] {
                                new SqlParameter("@P_drug_name" , model.search_name),
                                new SqlParameter("@P_drug_id" , -1)
                            };
                            data_warning = db.Database.SqlQuery<warning>("GET_drug_warnings @P_drug_name , @P_drug_id", par_active).ToList();
                        }

                        if (model.side_effects == true)
                        {
                            SqlParameter[] par_active = new SqlParameter[] {
                            new SqlParameter("@P_drug_name" , model.search_name),
                            new SqlParameter("@P_drug_id" , -1)
                            };
                            side_effect = db.Database.SqlQuery<side_effect>("GET_drug_side_effects @P_drug_name , @P_drug_id", par_active).ToList();
                        }

                        if ((data_desc == null || data_desc.Count == 0) &&
                            (data_dosage == null || data_dosage.Count == 0) &&
                            (data_warning == null || data_warning.Count == 0) &&
                            (side_effect == null || side_effect.Count == 0))
                        {
                            SqlParameter[] par_actives = new SqlParameter[] {
                            new SqlParameter("@name" , model.search_name)
                            };
                            actives = db.Database.SqlQuery<drug_trade_search_name>("get_active_for_trade @name", par_actives).ToList();
                        }

                        ViewBag.serchFor = null;
                        result.description = model.description;
                        result.drug_dosage = model.drug_dosage;
                        result.warnings = model.warnings;
                        result.side_effects = model.side_effects;
                        result.trade_names = model.trade_names;
                        result.name = model.search_name;
                        result.active_name = data_desc;
                        result.dosage = data_dosage;
                        result.warning = data_warning;
                        result.side_effect = side_effect;
                        result.active_name_for_trade = actives;
                        return View(result);
                    }
                }
            }
            return View(result);

        }






        public ActionResult Drug_Interactions(drug_interaction model)
        {
            ViewBag.serchFor = "no result yet";
            ViewBag.name = model.name;
            if (model.name != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Session["name_Interactions"] = model.name;
                    SqlParameter[] par_active = new SqlParameter[] {
                    new SqlParameter("@name" , model.name),
                    };
                    var data = db.Database.SqlQuery<drug_interaction>("interaction_search @name", par_active).ToList();
                    if (data.Count() == 0)
                    {
                        SqlParameter[] par_actives = new SqlParameter[] {
                            new SqlParameter("@name" , model.name)
                            };
                        var actives = db.Database.SqlQuery<drug_interaction>("get_active_for_trade @name", par_actives).ToList();
                        if (actives.Count() != 0)
                        {
                            Session["drug_Interactions"] = actives;
                            return View(actives);
                        }
                        else
                        {
                            var find = db.drug_active_names.Where(filter => filter.name == model.name).FirstOrDefault();
                            if (find != null)
                            {
                                List<drug_interaction> find_model_list = new List<drug_interaction>();
                                drug_interaction find_model = new drug_interaction();

                                find_model.active_without_interaction = find.name;
                                find_model_list.Add(find_model);

                                Session["drug_Interactions"] = find_model_list;
                                return View(find_model_list);
                            }
                        }
                    }
                    Session["drug_Interactions"] = data;
                    return View(data);
                }
            }
            else if (Session["drug_Interactions"] != null)
            {
                ViewBag.name = Session["name_Interactions"];
                return View(Session["drug_Interactions"]);
            }
            else if (!(Session["drug_Interactions"] != null))
            {
                Session.Abandon();
            }
            return View();
        }




        public ActionResult Drug_A_Z(string Letter)
        {
            Session.Abandon();
            if (Letter != null)
            {
                ViewBag.letter = Letter;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    SqlParameter[] letter = new SqlParameter[] {
                new SqlParameter("@letter" ,Letter )
                };
                    var names = db.Database.SqlQuery<Drug_a_zModelView>("Drug_a_z @letter", letter).ToList();
                    return View(names);
                }
            }
            return View();
        }

    }


}