#include <opencv2/core/core.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <string>
#include <iostream>
#include <cstdlib>

using namespace std;
using namespace cv;

const string trackbarWindowName = "Trackbars";
int H_MIN = 0;
int H_MAX = 256;
int S_MIN = 0;
int S_MAX = 256;
int V_MIN = 0;
int V_MAX = 256;

void on_trackbar( int, void* ) {
    //This function gets called whenever a
    // trackbar position is changed
}

void detect(const Mat inputImage, Mat& outputImage);
void detectRed(Mat hsvImage, Mat& redInImage);
void detectBlack(Mat hsvImage, Mat& blackInImage);
void detectBlue(Mat hsvImage, Mat& blueInImage);
void detectGreen(Mat hsvImage, Mat& greenInImage);
void detectYellow(Mat hsvImage, Mat& yellowInImage);

void redImageProc(Mat& redInImage);

void findCircleCircle(Mat& image, vector<Point>& foundPoints);


void createTrackbars();

int main() {
    namedWindow("Output Image");
    namedWindow("Detected Red");
    namedWindow("Detected Black");
    namedWindow("Detected Green");
    namedWindow("Detected Blue");
    namedWindow("Detected Yellow");
    namedWindow("Cropped Image");

    Size imageSize(640, 360);

    Mat inputImage;
    inputImage = imread("basic.png");

    resize(inputImage, inputImage, imageSize);

    //while(1) {
        Mat outputImage;

        detect(inputImage, outputImage);

        Mat initialColor(10, 10, CV_8UC3, Scalar(H_MIN, S_MIN, V_MIN));
        Mat endColor(10, 10, CV_8UC3, Scalar(H_MAX, S_MAX, V_MAX));

        cvtColor(initialColor, initialColor, COLOR_HSV2BGR);
        cvtColor(endColor, endColor, COLOR_HSV2BGR);

        imshow("Output Image", outputImage);
        waitKey(30);
    //}

    waitKey(0);
    return 0;
}

void createTrackbars() {
    namedWindow(trackbarWindowName, 0);
    createTrackbar( "H_MIN", trackbarWindowName, &H_MIN, H_MAX, on_trackbar );
    createTrackbar( "H_MAX", trackbarWindowName, &H_MAX, H_MAX, on_trackbar );
    createTrackbar( "S_MIN", trackbarWindowName, &S_MIN, S_MAX, on_trackbar );
    createTrackbar( "S_MAX", trackbarWindowName, &S_MAX, S_MAX, on_trackbar );
    createTrackbar( "V_MIN", trackbarWindowName, &V_MIN, V_MAX, on_trackbar );
    createTrackbar( "V_MAX", trackbarWindowName, &V_MAX, V_MAX, on_trackbar );
}

void morphOps(Mat &thresh){
    Mat erodeElement = getStructuringElement( MORPH_RECT,Size(3,3));
    Mat dilateElement = getStructuringElement( MORPH_RECT,Size(8,8));

    erode(thresh,thresh,erodeElement);
    erode(thresh,thresh,erodeElement);

    dilate(thresh,thresh,dilateElement);
    dilate(thresh,thresh,dilateElement);
}

void detect(Mat inputImage, Mat& outputImage) {
    //Blur the image a bit
    Mat blurImage;
    medianBlur(inputImage, blurImage, 3);

    //Convert Image to HSV
    Mat hsvImage;
    cvtColor(inputImage, hsvImage, COLOR_BGR2HSV);

    Mat redInImage;
    detectRed(hsvImage, redInImage);

    Mat blackInImage;
    detectBlack(hsvImage, blackInImage);

    Mat blueInImage;
    detectBlue(hsvImage, blueInImage);

    Mat greenInImage;
    detectGreen(hsvImage, greenInImage);

    Mat yellowInImage;
    detectYellow(hsvImage, yellowInImage);

    vector<Point> pointsRed;
    findCircleCircle(redInImage, pointsRed);

    vector<Point> pointsGreen;
    findCircleCircle(greenInImage, pointsGreen);

    bitwise_or(redInImage, blackInImage, outputImage);
    bitwise_or(blueInImage, outputImage, outputImage);
    bitwise_or(greenInImage, outputImage, outputImage);
    bitwise_or(yellowInImage, outputImage, outputImage);

    imshow("Detected Red", redInImage);
    imshow("Detected Black", blackInImage);
    imshow("Detected Blue", blueInImage);
    imshow("Detected Green", greenInImage);
    imshow("Detected Yellow", yellowInImage);
    cvtColor(outputImage, outputImage, COLOR_GRAY2BGR);
}

void detectRed(Mat hsvImage, Mat& redInImage) {
    //Threshold Lower Red in HSV
    Mat lowerRed;
    inRange(hsvImage, Scalar(0, 100, 100), Scalar(10, 255, 255), lowerRed);

    //Threshold Upper Red in HSV
    Mat upperRed;
    inRange(hsvImage, Scalar(160, 100, 100), Scalar(179, 255, 255), upperRed);

    //Red in Image
    bitwise_or(lowerRed, upperRed, redInImage);
}

void detectBlack(Mat hsvImage, Mat& blackInImage) {
    inRange(hsvImage, Scalar(0, 0, 0), Scalar(180, 255, 160), blackInImage);
}

void detectBlue(Mat hsvImage, Mat& blueInImage) {
    inRange(hsvImage, Scalar(85, 70, 70), Scalar(135, 255, 255), blueInImage);
}

void detectGreen(Mat hsvImage, Mat& greenInImage) {
    inRange(hsvImage, Scalar(35, 70, 70), Scalar(75, 255, 255), greenInImage);
}

void detectYellow(Mat hsvImage, Mat& yellowInImage) {
    inRange(hsvImage, Scalar(23, 70, 70), Scalar(35, 255, 255), yellowInImage);
}

void redImageProc(Mat& redInImage) {

}

void findCircleCircle(Mat& image, vector<Point>& foundPoints) {
    vector<Vec3f> circles;
    HoughCircles(image, circles, CV_HOUGH_GRADIENT, 1, image.rows/8, 100, 20, 0, 0);

    for(size_t current_circle = 0; current_circle < circles.size(); ++current_circle) {
        Point center(round(circles[current_circle][0]), round(circles[current_circle][1]));
        int radius = round(circles[current_circle][2]);
        //circle(image, center, radius, Scalar(80, 255, 255), 3);

        Rect rect;
        rect.x = center.x - radius;
        rect.y = center.y - radius;
        rect.width = radius*2 - 1;
        rect.height = radius*2 - 1;

        Mat croppedImage = image(rect);
        imshow("Cropped Image", croppedImage);

        vector<Vec3f> insideCircles;
        HoughCircles(croppedImage, insideCircles, CV_HOUGH_GRADIENT, 1, croppedImage.rows/8, 100, 20, 0, 0);
        if (!insideCircles.empty()) {
            cout << "Found a Circle-Circle at X: " << center.x << " - Y: " << center.y << endl;
            foundPoints.push_back(center);
        }

    }
}
