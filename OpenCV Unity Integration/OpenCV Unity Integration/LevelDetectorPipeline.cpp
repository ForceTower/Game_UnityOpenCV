#include <opencv2\core.hpp>
#include <opencv2\highgui.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\imgcodecs.hpp>
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

void TakePicture (int useReference);
void ImageEnhancement ();
void RedDarkSeparation ();
void BlueGreenYellowSeparation ();
void PrepareStartEndEnemies ();
void FindCircleCircle (Mat& image, vector<Point>& foundPoints, Rect& saved = Rect());
void FindCircle (Mat& image, vector<Point>& foundPoints);
void CountSelectedInImage (Mat image, int& number, vector<LevelElement>& fillVector, Rect ignore = Rect());

Size _ImageSize;		//Default Resize size

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

bool _FoundStart;
bool _FoundFinish;

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
	_ImageSize = Size (160, 90);

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

extern "C" void __declspec(dllexport) __stdcall DetectionPipeline (int& stateIn, int& stateOut, int useReference) {
	stateIn = _CurrentState;

	if (_CurrentState == -1) //Error State
		stateOut = -1;

	else if (_CurrentState == 1) //Image Aquisition
		TakePicture (useReference);

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
    namedWindow ("Black");
    imshow ("Black", _DarkImage);
	CountSelectedInImage (_DarkImage, number, _BlackPlatforms);
}

extern "C" void __declspec(dllexport) __stdcall SetupYellowPlatforms (int& number) {
    namedWindow ("Yellow");
    imshow ("Yellow", _YellowImage);
	CountSelectedInImage (_YellowImage, number, _YellowPlatforms);
}

extern "C" void __declspec(dllexport) __stdcall SetupBluePlatforms (int& number) {
    namedWindow ("Blue");
    imshow ("Blue", _BlueImage);
	CountSelectedInImage (_BlueImage, number, _BluePlatforms);
}

extern "C" void __declspec(dllexport) __stdcall SetupGreenPlatforms (int& number) {
    namedWindow ("Green");
    imshow ("Green", _GreenImage);
	CountSelectedInImage (_GreenImage, number, _GreenPlatforms, _GreenRect);
}

extern "C" void __declspec(dllexport) __stdcall SetupRedPlatforms (int& number) {
    namedWindow ("Red");
    imshow ("Red", _RedImage);
	CountSelectedInImage (_RedImage, number, _RedPlatforms, _RedRect);
}

extern "C" void __declspec(dllexport) __stdcall GetBlackPlatforms (LevelElement* platforms, int maxPlatformsCount, int& platformsCount) {
    //cout << "Called GetBlackPlatforms" << endl;
	for (size_t i = 0; i < _BlackPlatforms.size () && i < maxPlatformsCount; i++) {
        LevelElement e = _BlackPlatforms.at (i);
        //cout << "X: " << e.X << " - Y: " << e.Y << endl;
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

extern "C" bool __declspec(dllexport) __stdcall GetPlayerStartEnd (LevelElement* startEndPointer) {
    startEndPointer[0] = _PlayerStart;
    startEndPointer[1] = _PlayerFinish;

    return _FoundStart & _FoundFinish;
}

extern "C" void __declspec(dllexport) __stdcall NumberOfBlueEnemies (int& enemiesCount) {
    enemiesCount = _BlueEnemies.size ();
}

extern "C" void __declspec(dllexport) __stdcall GetBlueEnemies (LevelElement* enemies, int maxEnemiesCount, int& enemiesCount) {
    for (size_t i = 0; i < _BlueEnemies.size () && i < maxEnemiesCount; i++) {
        enemies[i] = _BlueEnemies.at (i);
    }

    enemiesCount = _BlueEnemies.size ();
}

void TakePicture (int useReference) {
    if (useReference == 0)
        _Capture >> _InputImage;
    else {
        cv::String name = cv::String ("C:/Users/joaop/Documents/FinalProject-CompVisual/basic.png");
        _InputImage = imread (name);
    }

	//Resizes the image
    namedWindow ("Original Image");
    imshow ("Original Image", _InputImage);
	resize (_InputImage, _InputImage, _ImageSize);
    namedWindow ("Resized Image");
    imshow ("Resized Image", _InputImage);

	_CurrentState = 2;
}

void ImageEnhancement () {
	//Blur Image a little
	medianBlur (_InputImage, _InputImage, 3);

	//Convert Image color to HSV
	cvtColor (_InputImage, _InputImage, COLOR_BGR2HSV);

	_CurrentState = 3;
}

void RedDarkSeparation () {
	// Red Detection occurs in 2 steps, Lower and Upper fields
	Mat lowerRed;
	inRange (_InputImage, Scalar (0, 100, 100), Scalar (10, 255, 255), lowerRed);

	//Threshold Upper Red in HSV
	Mat upperRed;
	inRange (_InputImage, Scalar (160, 100, 100), Scalar (179, 255, 255), upperRed);

	//Red in Image is the result of the OR operation between upper and lower hsv
	bitwise_or (lowerRed, upperRed, _RedImage);

	// Dark Detection
	inRange (_InputImage, Scalar (0, 0, 0), Scalar (180, 255, 80), _DarkImage);

	_CurrentState = 4;
}

void BlueGreenYellowSeparation () {
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
	HoughCircles (image, circles, CV_HOUGH_GRADIENT, 1, image.rows / 10, 100, 4, 0, 0);

	//For each circle detected
	for (size_t current_circle = 0; current_circle < circles.size (); ++current_circle) {
		Point center (round (circles[current_circle][0]), round (circles[current_circle][1])); //Find the center
		int radius = round (circles[current_circle][2]); //Calc the radius

		Rect rect; //Create a rectangle that will be used to crop the image
		rect.x = center.x - radius; //Setup the rectangle start points ans size 
		rect.y = center.y - radius;
		rect.width = radius * 2 - 1;
		rect.height = radius * 2 - 1;

        try {
            Mat croppedImage = image (rect); //Crop the image

            vector<Vec3f> insideCircles;
            //Apply Hough Transform im the cropped image to find circles inside the circle
            HoughCircles (croppedImage, insideCircles, CV_HOUGH_GRADIENT, 1, croppedImage.rows / 10, 100, 4, 0, 0);

            //If detected a circle inside a circle, insert it in the vector
            if (!insideCircles.empty ()) {
                foundPoints.push_back (center);
                save = rect;
                return; //Only one should be selected
            }
        }
        catch (const exception& ex) {
            //cout << "Exception :" << ex.what () << endl;
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
        _FoundStart = true;
    }
    else {
        _FoundStart = false;
    }

	if (_LevelEndPoints.size () > 0) {
		Point p = _LevelEndPoints.at (0);
		_PlayerFinish = LevelElement (p.x, p.y);
        _FoundFinish = true;
    }
    else {
        _FoundFinish = false;
    }

	Point temp;
	for (size_t i = 0; i < _LevelBlueEnemies.size (); i++) {
		temp = _LevelBlueEnemies.at (i);
		_BlueEnemies.push_back (LevelElement(temp.x, temp.y));
	}
}

void CountSelectedInImage (Mat image, int& number, vector<LevelElement>& fillVector, Rect ignore) {
	fillVector.clear ();
	int height = image.rows;
	int width = image.cols;

	int count = 0;

	for (int i = 0; i < height; i++) {
		for (int j = 0; j < width; j++) {
			if (image.at<uchar> (i, j) /*&& !ignore.contains(Point(i, j))*/) {
				count++;
                LevelElement element (i, j);
				fillVector.push_back (element);
                //cout << "In Element X: " << element.X << " and J: " << element.Y << endl;
			}
		}
	}

	number = count;
}

/*
int main () {
    int a = 0, b = 0;
    Init (a, b);
    b = 0;
    while (1) {
        if (b < 9)
            DetectionPipeline (a, b);
        else {
            int n;
            SetupBlackPlatforms (n);
            cout << "Detected Black: " << n << endl;
            LevelElement k[3800];
            int a;
            GetBlackPlatforms (k, n, a);
            //SetState (1);
            waitKey (1000);
            b = 1;
        }

        //cout << "A: " << a << " B: " << b << endl;

        waitKey (30);
    }
}*/
