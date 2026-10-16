@echo off
cd /d C:\Users\admin\MaPDF
.\gradlew.bat build -PbuildWithFrontend=true -x test -x spotlessApply -x spotlessCheck -x spotlessJava -x spotlessJavaApply -x spotlessGradle -x spotlessGradleApply -x spotlessYaml -x spotlessYamlApply
