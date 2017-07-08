#include <opencv2\core.hpp>
#include <opencv2\highgui.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\objdetect.hpp>
#include <iostream>
#include <stdio.h>

using namespace std;
using namespace cv;

struct Circle {
	Circle (int x, int y, int radius) : X (x), Y (y), Radius (radius) {}
	int X, Y, Radius;
};

struct Line {
	Line (int startX, int startY, int endX, int endY) : StartX (startX), StartY (startY), EndX (endX), EndY (endY) {}
	int StartX, StartY, EndX, EndY;
};

CascadeClassifier _faceCascade;
String _windowName = "Unity OpenCV Test";

VideoCapture _capture;
int _scale = 1;

extern "C" int __declspec(dllexport) __stdcall Init (int& outCameraWidth, int& outCameraHeight) {
	if (!_faceCascade.load ("lbpcascade_frontalface.xml"))
		return -1;

	_capture.open (0);
	if (!_capture.isOpened ())
		return -2;

	outCameraWidth = (int)_capture.get (CAP_PROP_FRAME_WIDTH);
	outCameraHeight = (int)_capture.get (CAP_PROP_FRAME_HEIGHT);

	return 0;
}

extern "C" void __declspec(dllexport) __stdcall Close () {
	_capture.release ();
}

extern "C" void __declspec(dllexport) __stdcall SetScale (int scale) {
	_scale = scale;
}

extern "C" void __declspec(dllexport) __stdcall DetectLines (Line* detectedLines, int maxLines, int& detectedLinesCount) {
	Mat frame;
	_capture >> frame;

	if (frame.empty ())
		return;

	Mat edges;
	Canny (frame, edges, 50, 150);
	Mat grayscale;
	cvtColor (edges, grayscale, COLOR_BGR2GRAY);


	vector<Vec2f> lines;
	HoughLines (grayscale, lines, 1, CV_PI / 180, 200);

	for (size_t i = 0; i < lines.size (); i++) {
		float rho = lines[i][0];
		float theta = lines[i][1];

		double a = cos (theta), b = sin (theta);
		double x0 = a*rho, y0 = b*rho;

		Point pt1 (cvRound (x0 + 1000 * (-b)), cvRound (y0 + 1000 * (a)));
		Point pt2 (cvRound (x0 - 1000 * (-b)), cvRound (y0 - 1000 * (a)));

		line (frame, pt1, pt2, Scalar (0, 0, 255), 3, 8);
	}

	imshow (_windowName, frame);

}

extern "C" void __declspec(dllexport) __stdcall Detect (Circle* outFaces, int maxOutFacesCount, int& outDetectedFacesCount) {
	Mat frame;
	_capture >> frame;

	if (frame.empty ())
		return;

	vector<Rect> faces;
	Mat grayscaleFrame;
	cvtColor (frame, grayscaleFrame, COLOR_BGR2GRAY);

	Mat resizedGray;
	resize (grayscaleFrame, resizedGray, Size (frame.cols / _scale, frame.rows / _scale));
	equalizeHist (resizedGray, resizedGray);

	_faceCascade.detectMultiScale (resizedGray, faces);

	for (size_t i = 0; i < faces.size (); i++) {
		Point center (_scale * (faces[i].x + faces[i].width / 2), _scale * (faces[i].y + faces[i].height / 2));

		ellipse (frame, center, Size (_scale * faces[i].width / 2, _scale * faces[i].height / 2), 0, 0, 360, Scalar (0, 0, 255), 4, 8, 0);

		outFaces[i] = Circle (faces[i].x, faces[i].y, faces[i].width / 2);
		outDetectedFacesCount++;

		if (outDetectedFacesCount == maxOutFacesCount)
			break;
	}

	imshow (_windowName, frame);
}