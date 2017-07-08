#include <opencv2\core.hpp>
#include <opencv2\highgui.hpp>
#include <opencv2\imgproc.hpp>
#include <vector>
#include <iostream>

using namespace std;
using namespace cv;

struct LevelElement {
	LevelElement (int x, int y) : X (x), Y (y) {}
	LevelElement () {}
	int X;
	int Y;
};

void TakePicture ();
void ImageEnhancement ();
void RedDarkSeparation ();
void BlueGreenYellowSeparation ();
void PrepareStartEndEnemies ();
void FindCircleCircle (Mat& image, vector<Point>& foundPoints, Rect& saved = Rect());
void FindCircle (Mat& image, vector<Point>& foundPoints);
void CountSelectedInImage (Mat image, int& number, vector<LevelElement> fillVector, Rect ignore = Rect());

Size _ImageSize;			//Default Resize size

VideoCapture _Capture;	//Camera Buffer
int _CurrentState;		//State Machine State

Mat _InputImage;		//InputImage received by any source
Mat _RedImage;			//Only Red
Mat _BlueImage;			//Only Blue
Mat _GreenImage;		//Only Green
Mat _YellowImage;		//Only Yellow
Mat _DarkImage;			//Only Dark (Black, Gray)

vector<Point> _LevelStartPoints;	//Level Player Start
vector<Point> _LevelEndPoints;		//Level Player Finish
vector<Point> _LevelBlueEnemies;	//Level Blue Enemies

Rect _GreenRect;		//Selected End
Rect _RedRect;			//Selected Start

LevelElement _PlayerStart;
LevelElement _PlayerFinish;
vector <LevelElement> _BlackPlatforms;
vector <LevelElement> _YellowPlatforms;
vector <LevelElement> _RedPlatforms;
vector <LevelElement> _BluePlatforms;
vector <LevelElement> _GreenPlatforms;
vector <LevelElement> _BlueEnemies;

extern "C" int __declspec(dllexport) __stdcall Init (int& outCameraWidth, int& outCameraHeight) {
	//Opens the Camera
	_Capture.open (0);

	//if error on opening camera
	if (!_Capture.isOpened ()) {
		_CurrentState = -1;
		return -2;
	}

	//Returns the camera image size
	outCameraWidth = (int)_Capture.get (CAP_PROP_FRAME_WIDTH);
	outCameraHeight = (int)_Capture.get (CAP_PROP_FRAME_HEIGHT);

	//Sets default resize
	_ImageSize = Size (640, 360);

	//Set State to Ready (Start)
	_CurrentState = 1;

	return 0;
}

extern "C" void __declspec(dllexport) __stdcall Close () {
	//Releases the camera
	_Capture.release ();

	_CurrentState = 0;
}

extern "C" void __declspec(dllexport) __stdcall SetState (int state) {
	_CurrentState = state;
}

extern "C" void __declspec(dllexport) __stdcall DetectionPipeline (int& stateIn, int& stateOut) {
	stateIn = _CurrentState;

	if (_CurrentState == -1) //Error State
		stateOut = -1;

	else if (_CurrentState == 1) //Image Aquisition
		TakePicture ();

	else if (_CurrentState == 2) //Image Enchancement 
		ImageEnhancement ();

	else if (_CurrentState == 3) //Colors Separation [Red and Dark]
		RedDarkSeparation ();

	else if (_CurrentState == 4) //Colors Separation [Blue, Green, Yellow]
		BlueGreenYellowSeparation ();

	else if (_CurrentState == 5) { //Level Player Start
		FindCircleCircle (_RedImage, _LevelStartPoints, _RedRect);
		_CurrentState = 6;
	}

	else if (_CurrentState == 6) { //Level Player Finish
		FindCircleCircle (_GreenImage, _LevelEndPoints, _GreenRect);
		_CurrentState = 7;
	}

	else if (_CurrentState == 7) { //Blue Enemies
		FindCircle (_BlueImage, _LevelBlueEnemies);
		_CurrentState = 8;
	}

	else if (_CurrentState == 8) {
		PrepareStartEndEnemies ();
		_CurrentState = 9;
	}

	stateOut = _CurrentState;
}

extern "C" void __declspec(dllexport) __stdcall SetupBlackPlatforms (int& number) {
	CountSelectedInImage (_DarkImage, number, _BlackPlatforms);
}

extern "C" void __declspec(dllexport) __stdcall SetupYellowPlatforms (int& number) {
	CountSelectedInImage (_YellowImage, number, _YellowPlatforms);
}

extern "C" void __declspec(dllexport) __stdcall SetupBluePlatforms (int& number) {
	CountSelectedInImage (_BlueImage, number, _BluePlatforms);
}

extern "C" void __declspec(dllexport) __stdcall SetupGreenPlatforms (int& number) {
	CountSelectedInImage (_GreenImage, number, _GreenPlatforms, _GreenRect);
}

extern "C" void __declspec(dllexport) __stdcall SetupRedPlatforms (int& number) {
	CountSelectedInImage (_RedImage, number, _RedPlatforms, _RedRect);
}

extern "C" void __declspec(dllexport) __stdcall GetBlackPlatforms (LevelElement* platforms, int maxPlatformsCount, int& platformsCount) {
	for (size_t i = 0; i < _BlackPlatforms.size () && i < maxPlatformsCount; i++) {
		platforms[i] = _BlackPlatforms.at (i);
	}

	platformsCount = _BlackPlatforms.size ();
}

extern "C" void __declspec(dllexport) __stdcall GetRedPlatforms (LevelElement* platforms, int maxPlatformsCount, int& platformsCount) {
	for (size_t i = 0; i < _RedPlatforms.size () && i < maxPlatformsCount; i++) {
		platforms[i] = _RedPlatforms.at (i);
	}

	platformsCount = _RedPlatforms.size ();
}

extern "C" void __declspec(dllexport) __stdcall GetGreenPlatforms (LevelElement* platforms, int maxPlatformsCount, int& platformsCount) {
	for (size_t i = 0; i < _GreenPlatforms.size () && i < maxPlatformsCount; i++) {
		platforms[i] = _GreenPlatforms.at (i);
	}

	platformsCount = _GreenPlatforms.size ();
}

extern "C" void __declspec(dllexport) __stdcall GetYellowPlatforms (LevelElement* platforms, int maxPlatformsCount, int& platformsCount) {
	for (size_t i = 0; i < _YellowPlatforms.size () && i < maxPlatformsCount; i++) {
		platforms[i] = _YellowPlatforms.at (i);
	}

	platformsCount = _YellowPlatforms.size ();
}

extern "C" void __declspec(dllexport) __stdcall GetBluePlatforms (LevelElement* platforms, int maxPlatformsCount, int& platformsCount) {
	for (size_t i = 0; i < _BluePlatforms.size () && i < maxPlatformsCount; i++) {
		platforms[i] = _BluePlatforms.at (i);
	}

	platformsCount = _BluePlatforms.size ();
}

inline void TakePicture () {
	_Capture >> _InputImage;

	//Resizes the image
	resize (_InputImage, _InputImage, _ImageSize);

	_CurrentState = 2;
}

inline void ImageEnhancement () {
	//Blur Image a little
	medianBlur (_InputImage, _InputImage, 3);

	//Convert Image color to HSV
	cvtColor (_InputImage, _InputImage, COLOR_BGR2HSV);

	_CurrentState = 3;
}

inline void RedDarkSeparation () {
	// Red Detection occurs in 2 steps, Lower and Upper fields
	Mat lowerRed;
	inRange (_InputImage, Scalar (0, 100, 100), Scalar (10, 255, 255), lowerRed);

	//Threshold Upper Red in HSV
	Mat upperRed;
	inRange (_InputImage, Scalar (160, 100, 100), Scalar (179, 255, 255), upperRed);

	//Red in Image is the result of the OR operation between upper and lower hsv
	bitwise_or (lowerRed, upperRed, _RedImage);

	// Dark Detection
	inRange (_InputImage, Scalar (0, 0, 0), Scalar (180, 255, 160), _DarkImage);

	_CurrentState = 4;
}

inline void BlueGreenYellowSeparation () {
	// Blue Detection
	inRange (_InputImage, Scalar (85, 70, 70), Scalar (135, 255, 255), _BlueImage);

	// Green Detection
	inRange (_InputImage, Scalar (35, 70, 70), Scalar (75, 255, 255), _GreenImage);

	// Yellow Detection
	inRange (_InputImage, Scalar (23, 70, 70), Scalar (35, 255, 255), _YellowImage);

	_CurrentState = 5;
}

void FindCircleCircle (Mat& image, vector<Point>& foundPoints, Rect& save) {
	foundPoints.clear ();

	vector<Vec3f> circles;
	//Aply Hough Transform to detect Circles
	HoughCircles (image, circles, CV_HOUGH_GRADIENT, 1, image.rows / 8, 100, 20, 0, 0);

	//For each circle detected
	for (size_t current_circle = 0; current_circle < circles.size (); ++current_circle) {
		Point center (round (circles[current_circle][0]), round (circles[current_circle][1])); //Find the center
		int radius = round (circles[current_circle][2]); //Calc the radius

		Rect rect; //Create a rectangle that will be used to crop the image
		rect.x = center.x - radius; //Setup the rectangle start points ans size 
		rect.y = center.y - radius;
		rect.width = radius * 2 - 1;
		rect.height = radius * 2 - 1;

		Mat croppedImage = image (rect); //Crop the image

		vector<Vec3f> insideCircles;
		//Apply Hough Transform im the cropped image to find circles inside the circle
		HoughCircles (croppedImage, insideCircles, CV_HOUGH_GRADIENT, 1, croppedImage.rows / 8, 100, 20, 0, 0);

		//If detected a circle inside a circle, insert it in the vector
		if (!insideCircles.empty ()) {
			foundPoints.push_back (center);
			save = rect;
			return; //Only one should be selected
		}
	}
}

void FindCircle (Mat& image, vector<Point>& foundPoints) {
	foundPoints.clear ();

	vector<Vec3f> circles;
	//Aply Hough Transform to detect Circles
	HoughCircles (image, circles, CV_HOUGH_GRADIENT, 1, image.rows / 8, 100, 20, 0, 0);

	//For each circle detected
	for (size_t current_circle = 0; current_circle < circles.size (); ++current_circle) {
		Point center (round (circles[current_circle][0]), round (circles[current_circle][1])); //Find the center
		
		foundPoints.push_back (center); //Save the center point of the circle
	}
}

void PrepareStartEndEnemies () {
	_BlueEnemies.clear ();

	if (_LevelStartPoints.size () > 0) {
		Point p = _LevelStartPoints.at (0);
		_PlayerStart = LevelElement (p.x, p.y);
	}

	if (_LevelEndPoints.size () > 0) {
		Point p = _LevelEndPoints.at (0);
		_PlayerFinish = LevelElement (p.x, p.y);
	}

	Point temp;
	for (size_t i = 0; i < _LevelBlueEnemies.size (); i++) {
		temp = _LevelBlueEnemies.at (i);
		_BlueEnemies.push_back (LevelElement(temp.x, temp.y));
	}
}

void CountSelectedInImage (Mat image, int& number, vector<LevelElement> fillVector, Rect ignore) {
	fillVector.clear ();
	int height = image.rows;
	int width = image.cols;

	int count = 0;

	for (int i = 0; i < height; i++) {
		for (int j = 0; j < width; j++) {
			if (image.at<uchar> (i, j) && !ignore.contains(Point(i, j))) {
				count++;
				fillVector.push_back (LevelElement (i, j));
			}
		}
	}

	number = count;
}