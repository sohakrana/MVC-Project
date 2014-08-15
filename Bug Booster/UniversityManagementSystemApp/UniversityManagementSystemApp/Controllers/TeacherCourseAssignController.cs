﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniversityManagementSystemApp.Models;

namespace UniversityManagementSystemApp.Controllers
{
    public class TeacherCourseAssignController : Controller
    {
        private UniversityDbContext db = new UniversityDbContext();
        //private double creditTakenn;
        //private double creditRemainingg;
        //private double selectedSubjectCreditt;

        // GET: /TeacherCourseAssign/
        public ActionResult Index()
        {
            var teachercourseassigns = db.TeacherCourseAssigns.Include(t => t.Course).Include(t => t.Department).Include(t => t.Teacher);
            return View(teachercourseassigns.ToList());
        }
        public ActionResult ViewCourseStatus()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code");
            var teachercourseassigns = db.TeacherCourseAssigns.Include(t => t.Course).Include(t => t.Department).Include(t => t.Teacher);
            return View(teachercourseassigns.ToList());
        }

        public PartialViewResult FilterByDepartment(int? departmentId)
        {
            if (departmentId != null)
            {
                var model = db.TeacherCourseAssigns.Include(c => c.Department).Include(c => c.Course).Include(c => c.Teacher).Where(s => s.Department.DepartmentId == departmentId);
                return PartialView("~/Views/Shared/_ViewCourseStatus.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Shared/_ViewCourseStatus.cshtml", db.TeacherCourseAssigns.Include(t => t.Course).Include(t => t.Department).Include(t => t.Teacher));
            }
        }
        
        // GET: /TeacherCourseAssign/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCourseAssign teachercourseassign = db.TeacherCourseAssigns.Find(id);
            if (teachercourseassign == null)
            {
                return HttpNotFound();
            }
            return View(teachercourseassign);
        }

        // GET: /TeacherCourseAssign/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code");
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "Name");
            return View();
        }

        // POST: /TeacherCourseAssign/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="TeacherCourseAssignId,DepartmentId,TeacherId,CourseId")] TeacherCourseAssign teachercourseassign)
        {
            if (ModelState.IsValid)
            {
                var creditTaken = db.Teachers.Where(t => t.TeacherId == teachercourseassign.TeacherId).Select(t => t.Credit).DefaultIfEmpty(0).Sum();
                var assignedCredit = db.TeacherCourseAssigns.Where(t => t.TeacherId == teachercourseassign.TeacherId).Include(c => c.Course).Where(c => c.CourseId == c.Course.CourseId).Select(c => c.Course.Credit).DefaultIfEmpty(0).Sum();
                var selectedCourse = db.Courses.FirstOrDefault(x => x.CourseId == teachercourseassign.CourseId);
                var selectedSubjectCreditt = selectedCourse.Credit;
                var remainingCredit = creditTaken - assignedCredit;
                
                if (selectedSubjectCreditt > remainingCredit)
                {
                    UpdateTeacherCredit(creditTaken + (selectedSubjectCreditt - remainingCredit), teachercourseassign.TeacherId);
                }
                UpdateCourseStatus(1, teachercourseassign.CourseId);
                db.TeacherCourseAssigns.Add(teachercourseassign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", teachercourseassign.CourseId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", teachercourseassign.DepartmentId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "Name", teachercourseassign.TeacherId);
            return View(teachercourseassign);
        }

        private void UpdateTeacherCredit(double teacherCurrentCredit, int teacherId)
        {
            Teacher selectedTeacher = new Teacher();
            selectedTeacher = db.Teachers.Where(id => id.TeacherId == teacherId).Single();
            selectedTeacher.Credit = teacherCurrentCredit;
            db.Entry(selectedTeacher).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void UpdateCourseStatus(int courseStatus, int courseId)
        {
            Course selectedCors = new Course();
            selectedCors = db.Courses.Where(id => id.CourseId == courseId).Single();
            selectedCors.CourseStatus = courseStatus;
            db.Entry(selectedCors).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: /TeacherCourseAssign/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCourseAssign teachercourseassign = db.TeacherCourseAssigns.Find(id);
            if (teachercourseassign == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", teachercourseassign.CourseId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", teachercourseassign.DepartmentId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "Name", teachercourseassign.TeacherId);
            return View(teachercourseassign);
        }

        // POST: /TeacherCourseAssign/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="TeacherCourseAssignId,DepartmentId,TeacherId,CourseId")] TeacherCourseAssign teachercourseassign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teachercourseassign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", teachercourseassign.CourseId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", teachercourseassign.DepartmentId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "Name", teachercourseassign.TeacherId);
            return View(teachercourseassign);
        }

        // GET: /TeacherCourseAssign/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCourseAssign teachercourseassign = db.TeacherCourseAssigns.Find(id);
            if (teachercourseassign == null)
            {
                return HttpNotFound();
            }
            return View(teachercourseassign);
        }

        // POST: /TeacherCourseAssign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeacherCourseAssign teachercourseassign = db.TeacherCourseAssigns.Find(id);
            db.TeacherCourseAssigns.Remove(teachercourseassign);
            db.SaveChanges();
            UpdateCourseStatus(0,teachercourseassign.CourseId);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult GetTeacherInfo(int teacherId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var creditTaken = db.Teachers.Where(t =>t.TeacherId == teacherId).Select(t =>t.Credit).DefaultIfEmpty(0).Sum();
            var assignedCredit = db.TeacherCourseAssigns.Where(t => t.TeacherId == teacherId).Include(c => c.Course).Where(c => c.CourseId == c.Course.CourseId).Select(c => c.Course.Credit).DefaultIfEmpty(0).Sum();
            
            var data = new {CreditTaken = creditTaken, CreditRemaining = creditTaken - assignedCredit};
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourseInfo(int courseId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var selectedCourse = db.Courses.FirstOrDefault(x => x.CourseId == courseId);
            return Json(selectedCourse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTeacherInfoByDept(int departmentId)
        {
            var teachers = db.Teachers.Where(td => td.DepartmentId == departmentId);

            return Json(new SelectList(teachers.AsQueryable(), "TeacherId", "Name"));
        }
    }
}
