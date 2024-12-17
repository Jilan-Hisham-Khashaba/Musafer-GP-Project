# face recognition part II
#IMPORT
import cv2 as cv
import numpy as np
import os
os.environ['TF_CPP_MIN_LOG_LEVEL']='2'
import tensorflow as tf
from sklearn.preprocessing import LabelEncoder
import pickle
from keras_facenet import FaceNet
#INITIALIZE
facenet = FaceNet()
faces_embeddings = np.load("faces_embeddings_done_4classes.npz")
Y = faces_embeddings['arr_1']
print(Y)

encoder = LabelEncoder()
encoder.fit(Y)
haarcascade = cv.CascadeClassifier("haarcascade_frontalface_default.xml")
model = pickle.load(open("svm_model_160x160 (2).pkl", 'rb'))

cap = cv.VideoCapture(0)

# WHILE LOOP

# ... (الكود الحالي)

# قيمة عتبة لتحديد متى يُعتبر الوجه "غريبًا"
threshold = 0.5  # يمكنك تعديل هذه القيمة حسب احتياجاتك

while cap.isOpened():
    _, frame = cap.read()
    rgb_img = cv.cvtColor(frame, cv.COLOR_BGR2RGB)
    gray_img = cv.cvtColor(frame, cv.COLOR_BGR2GRAY)
    faces = haarcascade.detectMultiScale(gray_img, 1.3, 5)

    for x, y, w, h in faces:
        img = rgb_img[y:y + h, x:x + w]
        img = cv.resize(img, (160, 160))
        img = np.expand_dims(img, axis=0)
        ypred = facenet.embeddings(img)
        face_name = model.predict(ypred)

        # حساب الاحتمالات باستخدام predict_proba
        ypreds_prob = model.predict_proba(ypred)[0]
        max_confidence = np.max(ypreds_prob)

        # شرط للتحقق مما إذا كان الوجه ينتمي إلى الأشخاص المعروفين
        if max_confidence >= threshold:
            matched_class = encoder.inverse_transform(face_name)[0]
            print(f"The face matches with class: {matched_class} with confidence: {max_confidence}")

            # عرض اسم الشخص على الفيديو
            cv.rectangle(frame, (x, y), (x + w, y + h), (255, 0, 255), 10)
            cv.putText(frame, str(matched_class), (x, y - 10), cv.FONT_HERSHEY_SIMPLEX,
                       1, (0, 0, 255), 3, cv.LINE_AA)
        else:
            # عرض "No Known" على الفيديو
            print("No Known")
            cv.rectangle(frame, (x, y), (x + w, y + h), (255, 0, 255), 10)
            cv.putText(frame, "No Known", (x, y - 10), cv.FONT_HERSHEY_SIMPLEX,
                       1, (0, 0, 255), 3, cv.LINE_AA)




    cv.imshow("Face Recognition:", frame)
    key = cv.waitKey(1) & 0xFF
    if key == ord('q'):
        break
cap.release()
cv.destroyAllWindows