# SS-API – SmartSeg Customer Segmentation API

**SmartSeg** is a modular customer segmentation platform built with **.NET** as a practice project for the SWE course.  
It enables data-driven teams to upload datasets, clean and encode data, select relevant features, apply KMeans clustering,  
and generate human-readable insights — all in a fully decoupled architecture.

---

## Key Features

- **Dataset Upload & Validation**  
  Upload CSV datasets and validate format, data types, and missing values.

- **Data Cleaning**  
  Handle nulls, clean structural problems, fix duplicates, etc.

- **Feature Engineering & Selection**  
  Use feature engineering methods to identify the optimal features for the segmentation algorithm,  
  normalize formats, and apply label encoding for categorical features.

- **Optimal K Estimation**  
  Use the Elbow method to recommend the best number of clusters.

- **KMeans Clustering Engine**  
  Apply Scikit-learn’s KMeans via Python integration to segment customers.

- **Readable Reporting**  
  Convert numeric cluster outputs into readable summaries (e.g., “Budget-conscious females aged 25–34”).  
  Analyze each cluster separately to find patterns and summaries.

---

## Project Structure

| Folder Name       | Description                                           |
|-------------------|-------------------------------------------------------|
| **DataHandling/** | Uploading, cleaning, and preprocessing                |
| **FeatureHandling/** | Encoding and feature selection/extraction           |
| **MachineLearning/** | Clustering logic and optimal K estimation           |
| **Reporting/**    | Interpreting and visualizing clusters                 |
