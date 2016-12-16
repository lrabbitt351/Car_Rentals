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
        private carRentalsContext _context; //create private class to hold our context object
        public carRentalsController(carRentalsContext context) //on controller instantiation...
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
                        admin = false,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    NewUser.password = Hasher.HashPassword(NewUser, NewUser.password); //hash the password
                    _context.Users.Add(NewUser); //add the user to the database
                    _context.SaveChanges(); //save the changes
                    currUser = _context.Users.SingleOrDefault(user => user.email == model.email); //retrieve the user from the database and store them in currUser (do this to get their user id)
                    return RedirectToAction("UserDash"); //return to the UserDash page
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
                    return RedirectToAction("UserDash"); //return to the UserDash page
                }
            }
            ViewBag.LogErrors = true; //unhide the errors on the login portion of the page
            return View("Index",model); //return the user to Index
        }

        [HttpGet]
        [Route("userdash")]
        public IActionResult UserDash()
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            List<Rental> current = new List<Rental>();
            List<Rental> overdue = new List<Rental>();
            List<Rental> rentals = _context.Rentals.Include(r => r.car).Where(r => r.userid == currUser.userid).ToList();
            foreach (Rental rental in rentals)
            {
                if (rental.return_at < DateTime.Now)
                {
                    overdue.Add(rental);
                }
                else
                {
                    current.Add(rental);
                }
            }
            ViewBag.name = currUser.first_name; //set ViewBag.name equal to the first name of the user set in currUser
            ViewBag.userID = currUser.userid; //set the user id to the user id of the current user
            ViewBag.admin = currUser.admin;
            ViewBag.current = current;
            ViewBag.overdue = overdue;
            return View("UserDash"); //return the Addquote page with the quote view model
        }

        [HttpGet]
        [Route("admindash")]
        public IActionResult AdminDash()
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            if (!currUser.admin)
            {
                return RedirectToAction("UserDash");
            }
            List<Car> onLot = _context.Cars.Where(c => c.inventory > 0).ToList();
            List<Rental> rented = _context.Rentals.Include(r => r.car).Include(r => r.user).ToList();
            ViewBag.name = currUser.first_name; //set ViewBag.name equal to the first name of the user set in currUser
            ViewBag.userID = currUser.userid; //set the user id to the user id of the current user
            ViewBag.admin = currUser.admin;
            ViewBag.onLot = onLot;
            ViewBag.rented = rented;
            return View("AdminDash"); //return the Addquote page with the quote view model
        }

        [HttpGet]
        [Route("plus")]
        public IActionResult Plus(int id)
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            if (!currUser.admin)
            {
                return RedirectToAction("UserDash");
            }
            Car car = _context.Cars.SingleOrDefault(c => c.carid == id);
            car.inventory += 1;
            car.updated_at = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("AdminDash");
        }

        [HttpGet]
        [Route("minus")]
        public IActionResult Minus(int id)
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            if (!currUser.admin)
            {
                return RedirectToAction("UserDash");
            }
            Car car = _context.Cars.SingleOrDefault(c => c.carid == id);
            car.inventory -= 1;
            car.updated_at = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("AdminDash");
        }

        [HttpGet]
        [Route("rent")]
        public IActionResult Rent()
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            List<Car> allCars = _context.Cars.Where(c => c.inventory > 0).ToList();
            ViewBag.cars = allCars;
            RentViewModel model = new RentViewModel
            {
                carid = 0,
                rented_at = DateTime.Today,
                return_at = DateTime.Today
            };
            return View("Rent", model);
        }
        
        [HttpPost]
        [Route("addrental")]
        public IActionResult AddRental(RentViewModel model) //receives a NewQuote object as an argument AS LONG AS ALL NAMES OF INPUTS AND MODEL KEYS MATCH!!
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Success
            }
            if (ModelState.IsValid) //if the data entered in the inputs meets the min requirements as set forth in models...
            {
                DateTime now = DateTime.Today;
                if (model.rented_at < now || model.return_at <= model.rented_at)
                {
                    ViewBag.Dateerror = "Please enter a valid date range.";
                    ViewBag.RentErrors2 = true;
                    ViewBag.cars = _context.Cars.Where(c => c.inventory > 0).ToList();
                    return View("Rent", model);
                }
                Rental NewRental = new Rental //create a new quote object based on the quoteviewmodel
                {
                    rented_at = model.rented_at,
                    return_at = model.return_at,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now,
                    userid = currUser.userid,
                    carid = model.carid
                };
                _context.Add(NewRental); //send the NewQuote object (with populated information) to context to add to the DB
                Car car = _context.Cars.SingleOrDefault(c => c.carid == model.carid);
                car.inventory -= 1;
                car.updated_at = DateTime.Now;
                _context.SaveChanges(); //save the changes to the DB
                return RedirectToAction("UserDash"); //return to the Quote page
            }
            ViewBag.RentErrors = true;
            List<Car> allCars = _context.Cars.Where(c => c.inventory > 0).ToList();
            ViewBag.cars = allCars;
            return View("Rent", model);
        }

        [HttpGet]
        [Route("return")]
        public IActionResult Return(int id)
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            Rental rental = _context.Rentals.SingleOrDefault(r => r.rentalid == id);
            Car car = _context.Cars.SingleOrDefault(c => c.carid == rental.carid);
            currUser.rentals.Remove(rental);
            car.rentals.Remove(rental);
            _context.Remove(rental);
            car.inventory += 1;
            car.updated_at = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("AdminDash");
        }

        [HttpGet]
        [Route("userprof/{id}")]
        public IActionResult UserProf(int id)
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            if (currUser.userid != id && !currUser.admin)
            {
                return RedirectToAction("UserDash");
            }
            User user = _context.Users
                .Include(u => u.rentals)
                    .ThenInclude(r => r.car)
                    .SingleOrDefault(u => u.userid == id);
            ViewBag.name = user.first_name;
            ViewBag.curradmin = currUser.admin;
            ViewBag.useradmin = user.admin;
            ViewBag.userID = user.userid;
            ViewBag.rentals = user.rentals;
            return View("UserProfile");
        }

        [HttpGet]
        [Route("makeadmin/{id}")]
        public IActionResult MakeAdmin(int id)
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            if (!currUser.admin || currUser.userid == id)
            {
                return RedirectToAction("UserDash");
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
            user.updated_at = DateTime.Now;
            _context.SaveChanges();
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
            if (!currUser.admin)
            {
                return RedirectToAction("UserDash");
            }
            return View("CreateCar",new NewCarViewModel()); //return the Addquote page with the quote view model
        }

        [HttpPost]
        [Route("addcar")]
        public IActionResult AddCar(NewCarViewModel model) //receive a Quote object as a parameter based on the information in the input fields
        {
            if (currUser == null) //if there is no user information stored in currUser...
            {
                return RedirectToAction("Index"); //return the user to Index
            }
            if (!currUser.admin)
            {
                return RedirectToAction("UserDash");
            }
            if (ModelState.IsValid) //if the information entered matches the min reqs in your models...
            {
                Car NewCar = new Car
                {
                    make = model.make,
                    model = model.carmodel,
                    inventory = model.inventory,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                _context.Add(NewCar);
                _context.SaveChanges(); //save the changes in the DB
                return RedirectToAction("AdminDash"); //return the user to the Quotes page
            }
            ViewBag.CarErrors = true; //unhide the error box on the quotes page
            return View("CreateCar", model); //return the user to the Update page with the arg of the quote id
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
