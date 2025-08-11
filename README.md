# SS-API

SmartSeg – Customer Segmentation API
SmartSeg is a modular, customer segmentation platform built with .NET for practice and as a Project for the SWE course.
It enables data-driven teams to upload datasets, clean and encode data, select relevant features, apply KMeans clustering, and generate human-readable insights — all in a fully decoupled architecture.

Key Features
Dataset Upload & Validation
Uploads CSV datasets and validates format, types, and missing values.

Data Cleaning
Handles nulls, cleans structural problems, fix duplicates, etc...

Feature Engineering and Selection
Uses Feature engineering methods to identify the optimal features for the segmentation algorithm like normalizing formats, and appling label encoding for categorical features.

Optimal K Estimation
Uses Elbow to recommend the best number of clusters.

KMeans Clustering Engine
Applies Scikit-learn’s KMeans via Python integration to segment customers.

Readable Reporting
Converts numeric cluster outputs into readable summaries (e.g., “Budget-conscious females aged 25-34”). It takes the clustered data and analyzes each cluster separately to find patterns and summaries.

Structure
DataHandling/ - Uploading, cleaning and preprocessing
FeatureHandling/ - encoding and Feature selection/extraction
MachineLearning/ - Clustering logic and optimal K
Reporting/ - Interpreting and visualizing clusters
