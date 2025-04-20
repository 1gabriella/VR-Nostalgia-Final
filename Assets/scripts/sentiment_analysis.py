from fastapi import FastAPI
from pydantic import BaseModel
from transformers import AutoTokenizer, AutoModelForSequenceClassification
import torch

app = FastAPI()

# Load your tokenizer and model
tokenizer = AutoTokenizer.from_pretrained("google-bert/bert-base-cased")
model = AutoModelForSequenceClassification.from_pretrained("google-bert/bert-base-cased", num_labels=2)

class SentimentRequest(BaseModel):
    text: str

@app.post("/predict_sentiment")
def predict_sentiment(request: SentimentRequest):
    inputs = tokenizer(request.text, return_tensors="pt", padding="max_length", truncation=True, max_length=128)
    outputs = model(**inputs)
    logits = outputs.logits
    prediction = torch.argmax(logits, dim=-1).item()
    sentiment = "Positive" if prediction == 1 else "Negative"
    # Optionally, add a neutral condition based on thresholds.
    return {"sentiment": sentiment}

