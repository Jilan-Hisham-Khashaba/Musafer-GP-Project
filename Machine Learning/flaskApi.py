from flask import Flask, request, jsonify
import cv2
import numpy as np
from keras_facenet import FaceNet
import pickle
from mtcnn import MTCNN
from scipy.spatial.distance import cosine

app = Flask(__name__)
model = pickle.load(open("svm_model_160x160 (2).pkl", 'rb'))
embedder = FaceNet()
detector = MTCNN()
threshold = 0.5
total_images = 0
total_matches = 0


def get_frame_from_camera():

    cap = cv2.VideoCapture(0)

    ret, frame = cap.read()

    cap.release()

    return frame


@app.route('/compare_faces', methods=['POST'])
def compare_faces():
    global total_images, total_matches

    # Decode the images from the request
    image1 = cv2.imdecode(np.frombuffer(request.files['image1'].read(), np.uint8), cv2.IMREAD_COLOR)
    image1 = cv2.cvtColor(image1, cv2.COLOR_BGR2RGB)

    # Get the image from the camera
    image2 = get_frame_from_camera()

    # Convert the camera image to RGB
    image2_rgb = cv2.cvtColor(image2, cv2.COLOR_BGR2RGB)

    # Detect faces in both images
    results1 = detector.detect_faces(image1)
    results2 = detector.detect_faces(image2_rgb)

    if len(results1) < 1 or len(results2) < 1:
        return jsonify({'error': 'No face detected in one of the images'})

    # Extract the faces from both images
    x1, y1, w1, h1 = results1[0]['box']
    face1 = image1[y1:y1 + h1, x1:x1 + w1]
    face1 = cv2.resize(face1, (160, 160))

    x2, y2, w2, h2 = results2[0]['box']
    face2 = image2[y2:y2 + h2, x2:x2 + w2]
    face2 = cv2.resize(face2, (160, 160))

    def get_embedding(face_img):
        face_img = face_img.astype('float32')
        face_img = np.expand_dims(face_img, axis=0)
        yhat = embedder.embeddings(face_img)
        return yhat[0]

    # Get embeddings for both faces
    embedding1 = get_embedding(face1)
    embedding2 = get_embedding(face2)

    # Compute the cosine similarity between the embeddings
    similarity = 1 - cosine(embedding1, embedding2)

    # Update the total number of images compared
    total_images += 1

    # Check if the similarity exceeds the threshold
    if similarity >= threshold:
        total_matches += 1
        Accuracy = round((total_matches / total_images) * 100, 2)
        return jsonify({'MatchStatus': 'Match', 'accuracy': Accuracy})
    else:
        Accuracy = round((total_matches / total_images) * 100, 2)
        return jsonify({'MatchStatus': 'No Match', 'accuracy': Accuracy})


if __name__ == '__main__':
    app.run(debug=True)
