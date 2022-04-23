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


public class Board : MonoBehaviour
{
    public Material defaultMaterial;
    public Material selectedMaterial;
    public NavMeshAgent agent;
    public GameObject movingpiece;
    public Animator anim;

    //public Vector2Int gridPointreceive;
    public int animstates;
    public bool fights = false;

    public GameObject targets;
    public Animator targetanim;


    public GameObject AddPieceforward(GameObject piece, int col, int row)
    {
        Vector2Int gridPoint = Geometry.GridPoint(col, row);
        
        GameObject newPiece = Instantiate(piece, Geometry.PointFromGrid(gridPoint), Quaternion.Euler(0,0,0), gameObject.transform);
        return newPiece;
    }

    public GameObject AddPiecebackward(GameObject piece, int col, int row)
    {
        Vector2Int gridPoint = Geometry.GridPoint(col, row);
        
        GameObject newPiece = Instantiate(piece, Geometry.PointFromGrid(gridPoint), Quaternion.Euler(0, 180, 0), gameObject.transform);
        return newPiece;
    }
    public void RemovePiece(GameObject piece)
    {
        Destroy(piece);
    }

    public void MovePiece(GameObject piece, Vector2Int gridPoint)
    {
        if (piece != null)
        {
            movingpiece = piece;
            //piece.transform.position = Geometry.PointFromGrid(gridPoint);
            agent = movingpiece.GetComponent<NavMeshAgent>();
            agent.destination = Geometry.PointFromGrid(gridPoint);
        }
        else
            return;
        

    }

    public void SelectPiece(GameObject piece)
    {
       //MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();
        //renderers.material = selectedMaterial;
    }

    public void DeselectPiece(GameObject piece)
    {
        //MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();
        //renderers.material = defaultMaterial;
    }


    public void PieceAnimation (GameObject piece,int animstate)
    {
        if (piece != null)
        {
            movingpiece = piece;
            anim = movingpiece.GetComponent<Animator>();
            animstates = animstate;
            anim.SetInteger("state", animstates);
        }
        else
            return;

    }

    public void BoardCapturePieceAt(int animstate)
    {
        
    }

    public void Capturetarget(GameObject target)
    {
        targets = target;
    }

    public void Fightenabled (bool fight)
    {
        fights = fight;
    }
    IEnumerator Event()
        
    {
        anim.SetInteger("state", 2);

        targetanim = targets.GetComponent<Animator>();
        targetanim.SetInteger("state", 3);

        yield return new WaitForSeconds(2);

        targetanim.SetInteger("state", 4);

        yield return new WaitForSeconds(1);

        //Destroy(targets);
        targets.SetActive(false);
        fights = false;
        anim.SetInteger("state", 0);

    }

    


    public void Update()
    {
        Debug.Log(targets);
        if (agent.enabled && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= 0)
        {
            anim = movingpiece.GetComponent<Animator>();
            anim.SetInteger("state", 0);
          
        }
        if (fights == true)
        {
            Debug.Log(agent.remainingDistance);
            if (agent.enabled && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance >1f)
            {
                anim.SetInteger("state", 1);
                targetanim = targets.GetComponent<Animator>();
                targetanim.SetInteger("state", 0);
            }
            else if (agent.enabled && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < 1f)
            {
                Debug.Log(animstates);
                StartCoroutine(Event());
            }
           


        }
       
        


        
        
    }


}
