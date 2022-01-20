using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace MatchUp
{
    /// <summary>Transactions represent a request and response pair identified by a unique string ID</summary>
    /// <remarks>
    /// Each request that is sent generates a Transaction with a unique transaction ID.
    /// When the matchmaking server response to a request it will include the ID in the response.
    /// When the response is received the transaction ID is used to look up the transaction
    /// so that it can be completed and the onResponse handler can be called.
    /// </remarks>
    public class Transaction
    {
        const string TAG = "Transaction: ";
        public string transactionID;
        public Message request;
        public string response;
        public bool isComplete;

        public Action<bool, Transaction> onResponse;
        public Coroutine timeoutProcess;

        public Transaction(string transactionID, Message request, Action<bool, Transaction> onResponse)
        {
            this.transactionID = transactionID;
            this.request = request;
            this.onResponse = onResponse;
        }

        public void Complete(string response)
        {
            isComplete = true;
            this.response = response;
            onResponse(true, this);
        }

        public void Failed(string response)
        {
            isComplete = true;
            this.response = response;
            onResponse(false, this);
        }

        public void Timeout()
        {
            Debug.LogError(TAG + "Timed out waiting for response from matchmaking server.");
            isComplete = true;
            onResponse(false, this);
        }

        public static string GenerateID()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int transactionIDLength = 16;
            var charList = Enumerable.Repeat(chars, transactionIDLength).Select(s => s[UnityEngine.Random.Range(0, s.Length)]);
            return new string(charList.ToArray());
        }
    }
}
