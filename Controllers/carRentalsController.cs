using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using carRentals.Models;

namespace carRentals.Controllers
{
    public class carRentalsController : Controller
    {
        private carRentalContext _context; //create private class to hold our context object
        public carRentalsController(carRentalContext context) //on controller instantiation...
        {
            _context = context; //populate the private context object with a services context object
        }
        private static User currUser = null; //user object to store user information for the logged in user

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.LogView = new LogViewModel(); //pass an empty LogViewModel to the front end
            ViewBag.RegView = new RegViewModel(); //pas an empty RegViewModel to the front end
            return View(); //render the index page
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegViewModel model)
        {
            if (ModelState.IsValid) //if the data entered in the inputs meets the min requirements as set forth in the User class in models...
            {
                User checkUser = _context.Users.SingleOrDefault(user => user.email == model.email); //attempt to retrieve a user based on the e-mail entered...
                if (checkUser != null) //if a user is retrieved based on the entered email address..
                {
                    ViewBag.DupeReg = "User already exists, please log in."; //pass this error to the front end
                    return View("Index"); //return the user to the Index page with error...
                }
                else //if a user is not retrieved...
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>(); //create a new PasswordHasher object
                    User NewUser = new User //create a new user object with the appropriate corresponding information from the view model
                    {
                        first_name = model.first_name,
                        last_name = model.last_name,
                        email = model.email,
                        password = model.password,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    NewUser.password = Hasher.HashPassword(NewUser, NewUser.password); //hash the password
                    _context.Users.Add(NewUser); //add the user to the database
                    _context.SaveChanges(); //save the changes
                    currUser = _context.Users.SingleOrDefault(user => user.email == model.email); //retrieve the user from the database and store them in currUser (do this to get their user id)
                    return RedirectToAction("Success"); //return to the Success page
                }
            }
            ViewBag.RegErrors = true; //set regerrors to true to display the error box
            return View("Index", model); //return the user to Index with the RegViewModel and errors
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LogViewModel model)
        {
            User checkUser = _context.Users.SingleOrDefault(user => user.email == model.logemail); //create a User object and attempt to retrieve user information based on the entered email address
            if (ModelState.IsValid) //if the data entered in the inputs meets the min requirements as set forth in models...
            {
                var Hasher = new PasswordHasher<User>(); //create a new object to check a hashed password against
                if (checkUser == null) //if a user is not retrieved based on the entered email address...
                {
                    ViewBag.DupeLog = "Username or password incorrect."; //store this error in ViewBag
                    return View("Index"); //send user with error back to the Index page
                }
                else if (Hasher.VerifyHashedPassword(checkUser, checkUser.password, model.logpassword) == 0) //if the password entered doesn't match the password in the DB...
                {
                    ViewBag.DupeLog = "Username or password incorrect."; //store this error in ViewBag
                    return View("Index"); //send user with error back to the Index page
                }
                else //if a user is retrieved and the password is correct...
                {
                    currUser = checkUser; //set currUser equal to the retrieved user
                    return RedirectToAction("Success"); //return to the Success page
                }
            }
            ViewBag.LogErrors = true; //unhide the errors on the login portion of the page
            return View("Index",model); //return the user to Index
        }
        
        [HttpGet]
        [Route("success")]
        public IActionResult Success()
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            if (currUser.admin)
            {
                return RedirectToAction("UserDash");
            }
            return RedirectToAction("AdminDash");
        }

        [HttpGet]
        [Route("userdash")]
        public IActionResult UserDash()
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            ViewBag.name = currUser.first_name; //set ViewBag.name equal to the first name of the user set in currUser
            ViewBag.userID = currUser.userid; //set the user id to the user id of the current user
            return View("AddQuote",new QuoteViewModel()); //return the Addquote page with the quote view model
        }

        [HttpGet]
        [Route("admindash")]
        public IActionResult AdminDash()
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            ViewBag.name = currUser.first_name; //set ViewBag.name equal to the first name of the user set in currUser
            ViewBag.userID = currUser.userid; //set the user id to the user id of the current user
            return View("AddQuote",new QuoteViewModel()); //return the Addquote page with the quote view model
        }

        [HttpGet]
        [Route("rent")]
        public IActionResult Rent()
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            List<Quote> allQuotes = _context.Quotes.Include(quote => quote.user).ToList(); //generate a list of Quote objects and store all quotes in the DB in it
            if (allQuotes.Count > 0) //if there are quotes in the DB...
            {
                ViewBag.quoteShow = true; //allow the quoteContainer to be viewed and change h1
                ViewBag.quotes = allQuotes; //send the quotes to the Quotes page in ViewBag
            }
            else //if there aren't any quotes in the DB yet...
            {
                ViewBag.quoteShow = false; //don't allow the quoteContainer to be viewed and set default h1
            }
            ViewBag.UserId = currUser.userid; //send the user id to the front end
            return View("Quotes");
        }
        
        [HttpPost]
        [Route("addrental")]
        public IActionResult AddRental(QuoteViewModel model) //receives a NewQuote object as an argument AS LONG AS ALL NAMES OF INPUTS AND MODEL KEYS MATCH!!
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Success"); //return the user to Success
            }
            if (ModelState.IsValid) //if the data entered in the inputs meets the min requirements as set forth in models...
            {
                Quote NewQuote = new Quote //create a new quote object based on the quoteviewmodel
                {
                    quotetext = model.quotetext,
                    userid = currUser.userid,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                _context.Add(NewQuote); //send the NewQuote object (with populated information) to context to add to the DB
                _context.SaveChanges(); //save the changes to the DB
                return RedirectToAction("Quotes"); //return to the Quote page
            }
            ViewBag.QuoteErrors = true; //show the quote errors notification box
            ViewBag.name = currUser.first_name; //set ViewBag.name equal to the first name of the user set in currUser
            ViewBag.userID = currUser.userid; //send the current user id to the front end in ViewBag
            return View("AddQuote", model); //return the user to Success
        }

        [HttpGet]
        [Route("userprof/{id}")]
        public IActionResult UserProf(int id)
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            var user = _context.Users
                .Include(u => u.rentals)
                    .ThenInclude(r => r.car)
                    .SingleOrDefault(u => u.userid == id);
            ViewBag.name = user.first_name;
            ViewBag.rentals = user.rentals;
            return View("UserProfile");
        }

        [HttpGet]
        [Route("madeadmin/{id}")]
        public IActionResult MadeAdmin(int id)
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            var user = _context.Users.SingleOrDefault(u => u.userid == id);
            if (!user.admin)
            {
                user.admin = true;
            }
            else
            {
                user.admin = false;
            }
            return RedirectToAction("UserProf", new {id = id});
        }

        [HttpGet]
        [Route("newcar")]
        public IActionResult NewCar()
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            return View("AddQuote",new QuoteViewModel()); //return the Addquote page with the quote view model
        }

        [HttpPost]
        [Route("addcar")]
        public IActionResult AddCar(UpquoteViewModel model) //receive a Quote object as a parameter based on the information in the input fields
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            if (currUser.userid != model.userid) { //if the user isn't the one who created the quote to update...
                return RedirectToAction("Quotes"); //return the user to the Quotes page
            }
            if (ModelState.IsValid) //if the information entered matches the min reqs in your models...
            {
                Quote upQuote =  _context.Quotes.SingleOrDefault(quote => quote.quoteid == model.quoteid); //retrieve the particular quote's information from the DB
                upQuote.quotetext = model.quotetext; //update the quote information based on the updated quote from the form
                upQuote.updated_at = DateTime.Now; //change the updated_at information for the quote
                _context.SaveChanges(); //save the changes in the DB
                return RedirectToAction("Quotes"); //return the user to the Quotes page
            }
            ViewBag.QuoteErrors = true; //unhide the error box on the quotes page
            return View("Update", model); //return the user to the Update page with the arg of the quote id
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            currUser = null; //reset currUser
            return RedirectToAction("Index"); //return the user to Index
        }
    }
}
