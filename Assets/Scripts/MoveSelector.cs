/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MoveSelector : MonoBehaviour
{
    public GameObject moveLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject attackLocationPrefab;

    private GameObject tileHighlight;
    public GameObject movingPiece;
    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;

    private Vector2Int gridPoint;
    public Board board;

    public GameObject target;

    IEnumerator Event()
    {
        board.Fightenabled(true);
;        board.PieceAnimation(movingPiece, 1);
        Debug.Log("run");
        //this.SendMessageUpwards("BoardCapturePieceAt", 1);
        GameManager.instance.Move(movingPiece, gridPoint);

        yield return new WaitForSeconds(2);

        //this.SendMessageUpwards("BoardCapturePieceAt", 2);
        board.PieceAnimation(movingPiece, 2);
        Debug.Log("fight");


        yield return new WaitForSeconds(3);

        board.PieceAnimation(movingPiece, 0);
        //this.SendMessageUpwards("BoardCapturePieceAt", 0);

        GameManager.instance.CapturePieceAt(gridPoint);
        board.Fightenabled(false);
        //ExitState();
    }

    void Start ()
    {
        this.enabled = false;
        tileHighlight = Instantiate(tileHighlightPrefab, Geometry.PointFromGrid(new Vector2Int(0, 0)),
            Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);
    }

    void Update ()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            gridPoint = Geometry.GridFromPoint(point);

            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);






            if (Input.GetMouseButtonDown(0))
            {
                // Reference Point 2: check for valid move location
                if (!moveLocations.Contains(gridPoint))
                {
                    return;
                }

                if (GameManager.instance.PieceAtGrid(gridPoint) == null)
                {
                    GameManager.instance.Move(movingPiece, gridPoint);
                    
                }
                else
                {
                    

                    GameObject pieceToCapture = GameManager.instance.PieceAtGrid(gridPoint);
                    board.Capturetarget(pieceToCapture);


                    GameManager.instance.Capturetarget(pieceToCapture);

                    GameManager.instance.Preparetocapture(gridPoint);

                    board.Fightenabled(true);
                    

                    //StartCoroutine(Event());


                     GameManager.instance.CapturePieceAt(gridPoint);

                     GameManager.instance.Move(movingPiece, gridPoint);

                }
                // Reference Point 3: capture enemy piece here later

                    ExitState();
                
            }
            
        }
        else
        {
            tileHighlight.SetActive(false);
        }
    }

    private void CancelMove()
    {
        this.enabled = false;

        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }

        GameManager.instance.DeselectPiece(movingPiece);
        TileSelector selector = GetComponent<TileSelector>();
        selector.EnterState();
    }

    public void EnterState(GameObject piece)
    {
        movingPiece = piece;
        this.enabled = true;

        moveLocations = GameManager.instance.MovesForPiece(movingPiece);
        Debug.Log("entermove");
        locationHighlights = new List<GameObject>();

        if (moveLocations.Count == 0)
        {
            CancelMove();
        }

        foreach (Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            if (GameManager.instance.PieceAtGrid(loc))
            {
                highlight = Instantiate(attackLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
                
            }
            else
            {
                highlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
                
            }
            locationHighlights.Add(highlight);
        }
    }

    private void ExitState()
    {
        this.enabled = false;
        TileSelector selector = GetComponent<TileSelector>();
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;
        GameManager.instance.NextPlayer();
        selector.EnterState();
        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
    }
}
