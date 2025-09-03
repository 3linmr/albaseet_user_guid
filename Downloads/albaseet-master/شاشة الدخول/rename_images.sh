#!/bin/bash

# سكريبت لتسمية جميع الصور Screenshot باسم المجلد الذي تحتويه

find . -name "Screenshot*" -type f | while read file; do
    # الحصول على اسم المجلد الأب
    dir=$(dirname "$file")
    dirname=$(basename "$dir")
    
    # تحويل المسافات إلى underscores
    newname=$(echo "$dirname" | tr ' ' '_')
    
    # إضافة رقم إذا كان هناك أكثر من صورة في نفس المجلد
    counter=1
    finalname="${newname}.png"
    
    while [ -f "$dir/$finalname" ]; do
        finalname="${newname}_${counter}.png"
        ((counter++))
    done
    
    echo "تسمية: $file -> $dir/$finalname"
    mv "$file" "$dir/$finalname"
done

echo "تم تسمية جميع الصور بنجاح!"
