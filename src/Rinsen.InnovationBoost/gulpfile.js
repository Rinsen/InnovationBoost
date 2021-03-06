﻿/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

/// <binding Clean='clean' />
"use strict";

var gulp = require('gulp'),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var webRoot = "./wwwroot/";
var webRootPaths = {
    js: webRoot + "js/debug/**/*.js",
    minJs: webRoot + "js/**/*.min.js",
    css: webRoot + "css/**/*.css",
    minCss: webRoot + "css/**/*.min.css",

    istevenMinJs: webRoot + "js/isteven-multi-select.min.js",
    concatJsDest: webRoot + "js/site.min.js",
    concatCssDest: webRoot + "css/site.min.css"
};

var npmPaths = {
    bootstrap: "./node_modules/bootstrap/dist/css/*.css",
    bootstrapIcons: "./node_modules/bootstrap-icons/icons/*.*",
    bootstrapJs: "./node_modules/bootstrap/dist/js/bootstrap*.js*",
    angularJs: "./node_modules/angular/*.js",
    angularSanitize: "./node_modules/angular-sanitize/*.js",
    jQuery: "./node_modules/jquery/dist/jquery*.js*",
    jQueryValidate: "./node_modules/jquery-validation/dist/jquery*.js*",
    jQueryValidateUnobtrusive: "./node_modules/jquery-validation-unobtrusive/dist/jquery*.js*",
    istevenMultiSelectCss: "./node_modules/isteven-angular-multiselect/*.css",
    istevenMultiSelectJs: "./node_modules/isteven-angular-multiselect/*.js",
    smartTableJs: "./node_modules/angular-smart-table/dist/*.js*",
    toastrJs: "./node_modules/toastr/toastr.js",
    toastrJsMin: "./node_modules/toastr/build/*.js*",
    toastrCss: "./node_modules/toastr/build/*.css*"
};

var destPaths = {
    css: webRoot + "/css/",
    icons: webRoot + "/icons/",
    js: webRoot + "/js/"
};

gulp.task("clean:js", function (cb) {
    rimraf(webRootPaths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(webRootPaths.concatCssDest, cb);
});

gulp.task("clean", gulp.parallel("clean:js", "clean:css"));

gulp.task("min:js", function () {
    return gulp.src([webRootPaths.js, "!" + webRootPaths.minJs], { base: "." })
        .pipe(concat(webRootPaths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([webRootPaths.css, "!" + webRootPaths.minCss])
        .pipe(concat(webRootPaths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("bootstrap", function () {
    gulp.src([npmPaths.bootstrap])
        .pipe(gulp.dest(destPaths.css));

    gulp.src([npmPaths.bootstrapIcons])
        .pipe(gulp.dest(destPaths.icons));

    return gulp.src([npmPaths.bootstrapJs])
        .pipe(gulp.dest(destPaths.js));
});

gulp.task("angular", function () {
    return gulp.src([npmPaths.angularJs])
        .pipe(gulp.dest(destPaths.js));
});

gulp.task("angularSanitize", function () {
    return gulp.src([npmPaths.angularSanitize])
        .pipe(gulp.dest(destPaths.js));
});

gulp.task("jQuery", function () {
    gulp.src([npmPaths.jQuery])
        .pipe(gulp.dest(destPaths.js));

    gulp.src([npmPaths.jQueryValidate])
        .pipe(gulp.dest(destPaths.js));

    return gulp.src([npmPaths.jQueryValidateUnobtrusive])
        .pipe(gulp.dest(destPaths.js));
});

gulp.task("istevenMultiSelect", function () {
    gulp.src([npmPaths.istevenMultiSelectJs])
        .pipe(concat(webRootPaths.istevenMinJs))
        .pipe(uglify())
        .pipe(gulp.dest("."));

    gulp.src([npmPaths.istevenMultiSelectJs])
        .pipe(gulp.dest(destPaths.js));

    return gulp.src([npmPaths.istevenMultiSelectCss])
        .pipe(gulp.dest(destPaths.css));
});

gulp.task("smartTable", function () {
    return gulp.src([npmPaths.smartTableJs])
        .pipe(gulp.dest(destPaths.js));
});

gulp.task("toastr", function () {
    gulp.src([npmPaths.toastrJs])
        .pipe(gulp.dest(destPaths.js));

    gulp.src([npmPaths.toastrJsMin])
        .pipe(gulp.dest(destPaths.js));

    return gulp.src([npmPaths.toastrCss])
        .pipe(gulp.dest(destPaths.css));
});

gulp.task("debug:ngApp", function () {
    return gulp.src("./ng-app/**/*.js")
        .pipe(gulp.dest("./wwwroot/js/debug/"));
});

gulp.task("3rdparty", gulp.parallel("bootstrap", "angular", "angularSanitize", "jQuery", "istevenMultiSelect", "smartTable", "toastr"));

gulp.task("min", gulp.parallel("min:js", "min:css"));