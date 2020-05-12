using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.Fast.SignalR;

namespace SignalR.Fast.Controllers
{
    public class SignalRController:Controller
    {
        IHubContext<SignalRHub> _context;
        public SignalRController(IHubContext<SignalRHub> context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Post()
        {
            
            
            return Ok();
        }
    }
}
