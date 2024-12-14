using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerControl : MonoBehaviour
{
    public TextMeshProUGUI Text;               // Step����r
    public TextMeshProUGUI Jump2Text;          // Jump_2����r
    public TextMeshProUGUI poopText;
    public GameObject CubePoop;                // �U���j�K��prefab
    public int Jump2MaxNum;                    // ��2�����W���A�]��public�A�i�bunity�����w
    public int poopMaxNum;

    private enum Element { Empty, Cube, Rock, Tree };  // �a�ϤW���󪺺���(�U�����j�K�ثe��Cube���)
    private Element[,,] field;                   // �a��
    private GameObject player;                   // �U����GameObject�A��ڲ��ʨϥ� 
    private GameObject[] cubes;                  // ��l�Ʀa�ϻݭn 
    private GameObject[] rocks;                  // ��l�Ʀa�ϻݭn
    private GameObject[] trees;                  // ��l�Ʀa�ϻݭn

    private int x = 0, y = 0, z = 0;              // player ����m
    private int x_max = 0, y_max = 0, z_max = 0;  // �a�Ϫ��d��
    private int dx = 0, dy = 0, dz = 0;           // ���Ჾ�ʪ��첾
    private float theta = 0;                      // ���ʫ᪺����
    private int step = 0;                         // �ثe���X�B
    private int poop_num = 0;
    private int jump2_num = 0;                    // ������2��������

    private AudioSource audioSource;              //sound
    public AudioClip moveSound;
    public AudioClip specialSound;

    public GameObject endpoint;                   //�P�_�q��
    GameObject animal;
    public GameObject animal_picture;
    public GameObject winUI;
    public GameObject gameoverUI;

    private float value = 0f;       // ��e�ƭ�
    private float value_y = 0f;
    private bool isMoving = false; // ����h��Ĳ�o
    private Animator animator;
    private Animator kangaroo_animator;

    public GameObject kangaroo;
    private bool isKangaroo = false;
    public GameObject bear;
    private bool isBear = false;


    // mouse raycast
    public GameObject hintPrefab;
    public GameObject rockHintPrefab;
    private List<GameObject> hintQueue = new List<GameObject>();
 
    private int movingDir;
    private bool isMovingRock;
    public Material hintMat;
    public Material pointingHintMat;

    void Start()
    {
        // ��l�ƳU����m
        player = GameObject.FindWithTag("Player");
        x = (int)player.transform.position.x;
        y = (int)player.transform.position.y;
        z = (int)player.transform.position.z;

        // ���o�a�Ϥj�p
        cubes = GameObject.FindGameObjectsWithTag("Cube");
        rocks = GameObject.FindGameObjectsWithTag("Rock");
        trees = GameObject.FindGameObjectsWithTag("Tree");
        foreach (GameObject obj in cubes)
        {
            int x_tmp = (int)obj.transform.position.x;
            int y_tmp = (int)obj.transform.position.y;
            int z_tmp = (int)obj.transform.position.z;
            if (x_tmp > x_max) x_max = x_tmp;
            if (y_tmp > y_max) y_max = y_tmp;
            if (z_tmp > z_max) z_max = z_tmp;
        }
        foreach (GameObject obj in rocks)
        {
            int x_tmp = (int)obj.transform.position.x;
            int y_tmp = (int)obj.transform.position.y;
            int z_tmp = (int)obj.transform.position.z;
            if (x_tmp > x_max) x_max = x_tmp;
            if (y_tmp > y_max) y_max = y_tmp;
            if (z_tmp > z_max) z_max = z_tmp;
        }
        foreach (GameObject obj in trees)
        {
            int x_tmp = (int)obj.transform.position.x;
            int y_tmp = (int)obj.transform.position.y;
            int z_tmp = (int)obj.transform.position.z;
            if (x_tmp > x_max) x_max = x_tmp;
            if (y_tmp > y_max) y_max = y_tmp;
            if (z_tmp > z_max) z_max = z_tmp;
        }

        // �[1�A�]���s��0-N�@��N+1��
        x_max++;
        y_max++;
        z_max++;

        // �o��y_max�A�[5�A�קK�U�����j�K�@�����|�A�W�L�쥻�a�Ϥj�p�C�o�̪�5�N��A�j�K�i�ﰪ�W�L�쥻�a��5��A�i�̷ӻݨD�ק�
        y_max += 5;

        // new �X �a�ϡA���k�s
        field = new Element[x_max, y_max, z_max];
        for (int i = 0; i < x_max; i++)
        {
            for (int j = 0; j < y_max; j++)
            {
                for (int k = 0; k < z_max; k++)
                {
                    field[i, j, k] = Element.Empty;
                }
            }
        }

        // �Ncube �M rock ��J�a��
        foreach (GameObject obj in cubes)
        {
            int x_tmp = (int)obj.transform.position.x;
            int y_tmp = (int)obj.transform.position.y;
            int z_tmp = (int)obj.transform.position.z;
            field[x_tmp, y_tmp, z_tmp] = Element.Cube;
        }
        foreach (GameObject obj in rocks)
        {
            int x_tmp = (int)obj.transform.position.x;
            int y_tmp = (int)obj.transform.position.y;
            int z_tmp = (int)obj.transform.position.z;
            field[x_tmp, y_tmp, z_tmp] = Element.Rock;
        }
        foreach (GameObject obj in trees)
        {
            int x_tmp = (int)obj.transform.position.x;
            int y_tmp = (int)obj.transform.position.y;
            int z_tmp = (int)obj.transform.position.z;
            field[x_tmp, y_tmp, z_tmp] = Element.Tree;
        }

        //��l�ƦU���B�ơB�ʪ���m
        GameObject game1 = GameObject.FindWithTag("game1");
        if (game1)
        {
            step = 21;
            animal = game1;
        }

        GameObject game2 = GameObject.FindWithTag("game2");
        if (game2)
        {
            step = 30;
            animal = game2;
        }

        GameObject game3 = GameObject.FindWithTag("game3");
        if (game3)
        {
            step = 35;
            animal = game3;
        }



        // �@�}�l��ܪ���r
        Text.text = "step:" + step.ToString();
        Jump2Text.text = "jump_2:" + (Jump2MaxNum - jump2_num).ToString();
        poopText.text = "poop:" + (poopMaxNum - poop_num).ToString();

        //music
        audioSource = gameObject.GetComponent<AudioSource>();

        //�P�_�q��
        endpoint = GameObject.FindGameObjectWithTag("endpoint");

        //�ʵe
        Transform childTransform = transform.Find("Wombat");
        animator = childTransform.GetComponent<Animator>();
        //Transform childTransform_kangaroo = transform.Find("kangaroo/Kangaroo");
        //kangaroo_animator = childTransform_kangaroo.GetComponent<Animator>();
    }
    private bool move()
    {
        if (!isMoving && !isBear && !isKangaroo)
        {
            dy = 0;
            if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                && y - 1 >= 0
                && field[x + dx, y, z + dz] == Element.Empty
                && (field[x + dx, y - 1, z + dz] == Element.Cube || field[x + dx, y - 1, z + dz] == Element.Rock))
            {
                step--;
                x += dx;
                y += dy;
                z += dz;
                transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                if (dx != 0)
                {
                    value = dx;
                    StartCoroutine(move_x());

                }
                else if (dz != 0)
                {
                    value = dz;
                    StartCoroutine(move_z());
                }

                //transform.position += new Vector3((float)dx, (float)dy, (float)dz);

                return true;
            }
        }
        return false;
    }
    private bool jump_1_level()
    { if (!isMoving && !isBear && !isKangaroo)
        {
            dy = 1;
            if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                && y + 1 < y_max
                && (field[x + dx, y, z + dz] == Element.Cube || field[x + dx, y, z + dz] == Element.Rock)
                && field[x + dx, y + 1, z + dz] == Element.Empty)
            {
                step--;
                x += dx;
                y += dy;
                z += dz;
                transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                if (dx != 0)
                {
                    value = dx;
                    StartCoroutine(jump1_x());
                }
                else if (dz != 0)
                {
                    value = dz;
                    StartCoroutine(jump1_z());
                }
                //transform.position += new Vector3((float)dx, (float)dy, (float)dz);
                return true;
            }
        }
        return false;
    }
    private bool jump_2_level()
    {
        if (!isMoving && isKangaroo && !isBear)
        {
            dy = 2;
            if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                && y + 1 < y_max
                && y + 2 < y_max
                && (field[x + dx, y, z + dz] == Element.Cube || field[x + dx, y, z + dz] == Element.Rock)
                && (field[x + dx, y + 1, z + dz] == Element.Cube || field[x + dx, y + 1, z + dz] == Element.Rock)
                && y + 2 < y_max && field[x + dx, y + 2, z + dz] == Element.Empty)
            {
                if (jump2_num >= Jump2MaxNum) return false;

                step--;
                jump2_num++;
                x += dx;
                y += dy;
                z += dz;

                if (dx != 0)
                {
                    value = dx;
                    StartCoroutine(jump2_x());
                } else if (dz != 0)
                {
                    value = dz;
                    StartCoroutine(jump2_z());
                }
                transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                kangaroo.transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);

                return true;
            }
        }
        return false;
    }
    private bool fall()
    {
        if (!isMoving && !isBear && !isKangaroo)
        {
            if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                && y - 1 >= 0
                && field[x + dx, y, z + dz] == Element.Empty
                && field[x + dx, y - 1, z + dz] == Element.Empty)
            {
                int next_y = y;
                while (next_y >= 0 && field[x + dx, next_y, z + dz] == 0) next_y--;
                dy = (next_y + 1) - y;

                step--;
                x += dx;
                y += dy;
                z += dz;
                transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                //transform.position += new Vector3((float)dx, (float)dy, (float)dz);
                if (dx != 0)
                {
                    value = dx;
                    StartCoroutine(fall1_x(Mathf.Abs(dy)));
                }
                else if (dz != 0) {
                    value = dz;
                    StartCoroutine(fall1_z(Mathf.Abs(dy)));
                }
                //else { transform.position += new Vector3((float)dx, (float)dy, (float)dz); }
                return true;
            }
        }
        return false;
    }
    private bool poop()
    {
        if (!isMoving && !isBear && !isKangaroo)
        {
            if (poop_num >= poopMaxNum) return false;
            GameObject prefab = Instantiate(CubePoop, transform.position, Quaternion.identity);
            field[x, y, z] = Element.Cube;
            dx = 0;
            dy = 1;
            dz = 0;
            //step--;
            poop_num++;
            x += dx;
            y += dy;
            z += dz;
            transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
            transform.position += new Vector3((float)dx, (float)dy, (float)dz);
        }
        return true;
    }
    private bool move_rock()
    {
        if (!isMoving && isBear)
        {
            dy = 0;
            float rock_dy = 0;
            if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                && x + dx * 2 >= 0 && x + dx * 2 < x_max && z + dz * 2 >= 0 && z + dz * 2 < z_max
                && y - 1 >= 0
                && field[x + dx, y, z + dz] == Element.Rock
                && field[x + dx * 2, y, z + dz * 2] == Element.Empty
                )
            {
                print("dx->"+dx);
                print("dz->"+dz);

                print("2->"+field[x + dx * 2, y, z + dz * 2]);
                print("inddorr");
                if (field[x + dx * 2, y - 1, z + dz * 2] != Element.Cube
                && field[x + dx * 2, y - 2, z + dz * 2] != Element.Cube) { rock_dy = -2; }
                Debug.Log(dy);
                RaycastHit hit;
                LayerMask mask;
                mask = ~(1 << 6);
                if (!Physics.Raycast(transform.position, new Vector3((float)dx, (float)dy, (float)dz), out hit, Mathf.Infinity, mask))
                {
                    print("raycasthit error");
                }
                if (hit.collider.tag != "Rock")
                {
                    print("tag error");
                }
                field[x + dx, y, z + dz] = Element.Empty;
                field[x + dx * 2, y + (int)rock_dy, z + dz * 2] = Element.Rock;
                step--;
                x += dx;
                y += dy;
                z += dz;
                if (rock_dy == -2)
                {
                    transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                    transform.position += new Vector3((float)dx, (float)dy, (float)dz);
                    bear.transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                    bear.transform.position += new Vector3((float)dx, (float)dy, (float)dz);
                    hit.collider.transform.position += new Vector3((float)dx, (float)dy + rock_dy, (float)dz);
                    StartCoroutine(WaitSecond_bear());
                }
                else if (rock_dy == 0) {
                    if (dx != 0)
                    {
                        value = dx;
                        StartCoroutine(moveRock_x(hit));
                        transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                        bear.transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                    }
                    else if (dz != 0)
                    {
                        value = dz;
                        StartCoroutine(moveRock_z(hit));
                        transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                        bear.transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
                    }
                }

                return true;
            }
        }
        return false;
    }
    private bool getMoveKey()
    {
        if (Input.GetKeyDown(KeyCode.W) || (Input.GetMouseButtonDown(0) && movingDir == 0))
        {
            dx = 1; dz = 0; theta = 90.0f;
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.S) || (Input.GetMouseButtonDown(0) && movingDir == 1))
        {
            dx = -1; dz = 0; theta = 270.0f;
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.A) || (Input.GetMouseButtonDown(0) && movingDir == 2))
        {
            dx = 0; dz = 1; theta = 0.0f;
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.D) || (Input.GetMouseButtonDown(0) && movingDir == 3))
        {
            dx = 0; dz = -1; theta = 180.0f;
            return true;
        }

        return false;
    }
    private void detectWin()
    {
        if (gameObject.transform.position == endpoint.transform.position)
        {
            GameObject isActive = GameObject.FindWithTag("picture");
            if (isActive != null)
            {
                Debug.Log("win");
                endpoint.SetActive(false);
                winUI.SetActive(true);
                gameObject.SetActive(false);
                //���q������
            }
        }
    }
    private void deteceGameover()
    {
        if (step == -1)
        {
            Debug.Log("gameover");
            gameoverUI.SetActive(true);
            gameObject.SetActive(false);
            //�����ѭ���
        }
    }
    private void detectSave()
    {
        if (gameObject.transform.position == animal.transform.position)
        {
            animal.gameObject.SetActive(false);
            animal_picture.SetActive(true);

        }
    }
    private IEnumerator move_x()
    {
        isMoving = true; // �����Ĳ�o
        animator.SetBool("move", isMoving);
        float startValue = transform.position.x;
        float endValue = transform.position.x + value;
        float duration = 1f; // �����L�窺�ɶ�
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            value = Mathf.Lerp(startValue, endValue, elapsed / duration);

            // �N�ƭ����Ω󨤦��m�]�ȧ@�d�ҡ^
            transform.position = new Vector3(value, transform.position.y, transform.position.z);

            yield return null; // ���ݤU�@�V
        }

        isMoving = false;
        animator.SetBool("move", isMoving);
    }
    private IEnumerator move_z()
    {
        isMoving = true; // �����Ĳ�o
        animator.SetBool("move", isMoving);
        float startValue = transform.position.z;
        float endValue = transform.position.z + value;
        float duration = 1f; // �����L�窺�ɶ�
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            value = Mathf.Lerp(startValue, endValue, elapsed / duration);

            // �N�ƭ����Ω󨤦��m�]�ȧ@�d�ҡ^
            transform.position = new Vector3(transform.position.x, transform.position.y, value);

            yield return null; // ���ݤU�@�V
        }

        isMoving = false;
        animator.SetBool("move", isMoving);
    }
    private IEnumerator jump1_x()
    {
        isMoving = true; // �����Ĳ�o
        animator.SetBool("jump", isMoving);
        float startValue_x = transform.position.x;
        float endValue_x = transform.position.x + value;
        float startValue_y = transform.position.y;
        float endValue_y = transform.position.y + 1;

        float duration = 0.5f; // �����L�窺�ɶ�
        float elapsed = 0f;

        float value_x;
        float value_y;

        gameObject.transform.rotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            value_y = Mathf.Lerp(startValue_y, endValue_y - 0.2f, elapsed / duration);

            transform.position = new Vector3(transform.position.x, value_y, transform.position.z);

            yield return null; // ���ݤU�@�V
        }
        gameObject.transform.rotation *= Quaternion.Euler(90.0f, 0.0f, 0.0f);
        transform.position = new Vector3(transform.position.x, endValue_y, transform.position.z);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            value_x = Mathf.Lerp(startValue_x, endValue_x, elapsed / duration);

            transform.position = new Vector3(value_x, transform.position.y, transform.position.z);

            yield return null; // ���ݤU�@�V
        }

        isMoving = false;
        animator.SetBool("jump", isMoving);
    }
    private IEnumerator jump1_z()
    {
        isMoving = true; // �����Ĳ�o
        animator.SetBool("jump", isMoving);
        float startValue_z = transform.position.z;
        float endValue_z = transform.position.z + value;
        float startValue_y = transform.position.y;
        float endValue_y = transform.position.y + 1;
        float duration = 0.5f; // �����L�窺�ɶ�
        float elapsed = 0f;

        float value_z;
        float value_y;
        gameObject.transform.rotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            value_y = Mathf.Lerp(startValue_y, endValue_y - 0.2f, elapsed / duration);

            transform.position = new Vector3(transform.position.x, value_y, transform.position.z);

            yield return null; // ���ݤU�@�V
        }
        gameObject.transform.rotation *= Quaternion.Euler(90.0f, 0.0f, 0.0f);
        transform.position = new Vector3(transform.position.x, endValue_y, transform.position.z);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            value_z = Mathf.Lerp(startValue_z, endValue_z, elapsed / duration);

            transform.position = new Vector3(transform.position.x, transform.position.y, value_z);

            yield return null; // ���ݤU�@�V
        }
        value = endValue_z; // �T�O��F�ؼЭ�
        isMoving = false;
        animator.SetBool("jump", isMoving);
    }
    private IEnumerator fall1_x(int dy)
    {
        isMoving = true; // �����Ĳ�o
        animator.SetBool("fall", isMoving);

        float startValue_x = transform.position.x;
        float endValue_x = transform.position.x + value;
        float startValue_y = transform.position.y;
        float endValue_y = transform.position.y - dy;

        float duration = 1f; // �����L�窺�ɶ�
        float elapsed = 0f;

        float value_x;
        float value_y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            value_x = Mathf.Lerp(startValue_x, (startValue_x + endValue_x) / 2, elapsed / duration);

            transform.position = new Vector3(value_x, transform.position.y, transform.position.z);

            yield return null; // ���ݤU�@�V
        }
        gameObject.transform.rotation *= Quaternion.Euler(90.0f, 0.0f, 0.0f);
        transform.position = new Vector3(endValue_x, transform.position.y, transform.position.z);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            value_y = Mathf.Lerp(startValue_y, endValue_y, elapsed / duration);

            transform.position = new Vector3(transform.position.x, value_y, transform.position.z);

            yield return null; // ���ݤU�@�V
        }

        gameObject.transform.rotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        isMoving = false;
        animator.SetBool("fall", isMoving);
    }
    private IEnumerator fall1_z(int dy)
    {
        isMoving = true; // �����Ĳ�o
        animator.SetBool("fall", isMoving);

        float startValue_z = transform.position.z;
        float endValue_z = transform.position.z + value;
        float startValue_y = transform.position.y;
        float endValue_y = transform.position.y - dy;

        float duration = 1f; // �����L�窺�ɶ�
        float elapsed = 0f;

        float value_z;
        float value_y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            value_z = Mathf.Lerp(startValue_z, (startValue_z + endValue_z) / 2, elapsed / duration);

            transform.position = new Vector3(transform.position.x, transform.position.y, value_z);

            yield return null; // ���ݤU�@�V
        }
        gameObject.transform.rotation *= Quaternion.Euler(90.0f, 0.0f, 0.0f);
        transform.position = new Vector3(transform.position.x, transform.position.y, endValue_z);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            value_y = Mathf.Lerp(startValue_y, endValue_y, elapsed / duration);

            transform.position = new Vector3(transform.position.x, value_y, transform.position.z);

            yield return null; // ���ݤU�@�V
        }

        gameObject.transform.rotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        isMoving = false;
        animator.SetBool("fall", isMoving);
    }
    private IEnumerator WaitSecond_kangaroo()
    {
        yield return new WaitForSeconds(0.2f);
        kangaroo.SetActive(false);
        transform.localScale = new Vector3(1f, 1f, 1f);
        isKangaroo = false;
    }
    private IEnumerator WaitSecond_bear()
    {
        yield return new WaitForSeconds(0.2f);
        //bear.SetActive(false);
        //transform.localScale = new Vector3(1f, 1f, 1f);
        //isBear = false;
    }
    public void jump2Button()
    {
        if (!isKangaroo && jump2_num < Jump2MaxNum)
        {
            isKangaroo = true;
            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            kangaroo.SetActive(true);
            kangaroo.transform.position = transform.position;
            kangaroo.transform.rotation = transform.rotation;
        } else if (isKangaroo) {
            isKangaroo = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
            kangaroo.SetActive(false);
        }


    }
    private IEnumerator jump2_x() {
        isMoving = true; // �����Ĳ�o
        //kangaroo_animator.SetBool("jump", isMoving);
        float startValue_x = transform.position.x;
        float endValue_x = transform.position.x + value;
        float startValue_y = transform.position.y;
        float endValue_y = transform.position.y + 2;

        float duration = 0.5f; // �����L�窺�ɶ�
        float elapsed = 0f;

        float value_x;
        float value_y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            value_y = Mathf.Lerp(startValue_y, endValue_y, elapsed / duration);

            kangaroo.transform.position = new Vector3(transform.position.x, value_y, transform.position.z);
            transform.position = new Vector3(transform.position.x, value_y, transform.position.z);
            yield return null; // ���ݤU�@�V
        }
        kangaroo.transform.position = new Vector3(endValue_x, transform.position.y, transform.position.z);
        transform.position = new Vector3(endValue_x, transform.position.y, transform.position.z);

        StartCoroutine(WaitSecond_kangaroo());


        isMoving = false;
        //kangaroo_animator.SetBool("jump", isMoving);
    }
    private IEnumerator jump2_z()
    {
        isMoving = true; // �����Ĳ�o
        //kangaroo_animator.SetBool("jump", isMoving);
        float startValue_z = transform.position.z;
        float endValue_z = transform.position.z + value;
        float startValue_y = transform.position.y;
        float endValue_y = transform.position.y + 2;

        float duration = 0.5f; // �����L�窺�ɶ�
        float elapsed = 0f;

        float value_z;
        float value_y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            value_y = Mathf.Lerp(startValue_y, endValue_y, elapsed / duration);

            kangaroo.transform.position = new Vector3(transform.position.x, value_y, transform.position.z);
            transform.position = new Vector3(transform.position.x, value_y, transform.position.z);
            yield return null; // ���ݤU�@�V
        }
        kangaroo.transform.position = new Vector3(transform.position.x, transform.position.y, endValue_z);
        transform.position = new Vector3(transform.position.x, transform.position.y, endValue_z);

        StartCoroutine(WaitSecond_kangaroo());


        isMoving = false;
        //kangaroo_animator.SetBool("jump", isMoving);
    }
    public void moveRockButton()
    {
        if (!isBear)
        {
            isBear = true;
            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            bear.SetActive(true);
            bear.transform.position = transform.position;
            bear.transform.rotation = transform.rotation;
        } else if (isBear)
        {
            isBear = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
            bear.SetActive(false);
        }
    }
    private IEnumerator moveRock_x(RaycastHit hit)
    {
        isMoving = true; // �����Ĳ�o
        float startValue_hit = hit.transform.position.x;
        float endValue_hit = hit.transform.position.x + value;
        float startValue = transform.position.x;
        float endValue = transform.position.x + value;
        float duration = 1f; // �����L�窺�ɶ�
        float elapsed = 0f;
        float value_hit;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            value_hit = Mathf.Lerp(startValue_hit, endValue_hit, elapsed / duration);
            value = Mathf.Lerp(startValue, endValue, elapsed / duration);

            // �N�ƭ����Ω󨤦��m�]�ȧ@�d�ҡ^
            hit.transform.position = new Vector3(value_hit, hit.transform.position.y, hit.transform.position.z);
            bear.transform.position = new Vector3(value, transform.position.y, transform.position.z);
            transform.position = new Vector3(value, transform.position.y, transform.position.z);

            yield return null; // ���ݤU�@�V
        }
        StartCoroutine(WaitSecond_bear());
        isMoving = false;
    }
    private IEnumerator moveRock_z(RaycastHit hit)
    {
        isMoving = true; // �����Ĳ�o
        float startValue_hit = hit.transform.position.z;
        float endValue_hit = hit.transform.position.z + value;
        float startValue = transform.position.z;
        float endValue = transform.position.z + value;
        float duration = 1f; // �����L�窺�ɶ�
        float elapsed = 0f;
        float value_hit;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            value_hit = Mathf.Lerp(startValue_hit, endValue_hit, elapsed / duration);
            value = Mathf.Lerp(startValue, endValue, elapsed / duration);

            // �N�ƭ����Ω󨤦��m�]�ȧ@�d�ҡ^
            hit.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, value_hit);
            bear.transform.position = new Vector3(transform.position.x, transform.position.y, value);
            transform.position = new Vector3(transform.position.x, transform.position.y, value);

            yield return null; // ���ݤU�@�V
        }
        StartCoroutine(WaitSecond_bear());
        isMoving = false;
    }
    private void nextStepHint()
    {
        // clear previous hintQueue
        if (hintQueue.Count > 0)
        {
            foreach (var tmp in hintQueue)
            {
                Destroy(tmp);
            }
        }
        hintQueue.Clear();

        int dx, dy, dz = 0; // 歸零
        int[,] offset = { {-1, 0},
                          { 1, 0},
                          { 0,-1},
                          { 0, 1} };

        if (!isMoving && !isKangaroo && !isBear)
        {
            // move
            for (int i = 0; i < 4; i++)
            {
                dx = offset[i, 0]; dz = offset[i, 1];
                if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                && y - 1 >= 0
                && field[x + dx, y, z + dz] == Element.Empty
                && (field[x + dx, y - 1, z + dz] == Element.Cube || field[x + dx, y - 1, z + dz] == Element.Rock))
                {
                    GameObject tmp = Instantiate(hintPrefab, new Vector3(x + dx, y - 1, z + dz), Quaternion.identity);
                    //print("move hint: (" + (x + dx) + ", " + (y - 1) + ", " + (z + dz) + ")");
                    hintQueue.Add(tmp);
                }
            }
            // jump 1 level
            dx = 0; dy = 0; dz = 0; // 歸零
            for (int i = 0; i < 4; i++)
            {
                dx = offset[i, 0]; dz = offset[i, 1];
                if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                && y + 1 < y_max
                && (field[x + dx, y, z + dz] == Element.Cube || field[x + dx, y, z + dz] == Element.Rock)
                && field[x + dx, y + 1, z + dz] == Element.Empty)
                {
                    GameObject tmp = Instantiate(hintPrefab, new Vector3(x + dx, y, z + dz), Quaternion.identity);
                    //print("jump 1 level hint: (" + (x + dx) + ", " + (y) + ", " + (z + dz) + ")");
                    hintQueue.Add(tmp);
                }
            }
            // fall
            dx = 0; dy = 0; dz = 0; // 歸零
            for (int i = 0; i < 4; i++)
            {
                dx = offset[i, 0]; dz = offset[i, 1];
                if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                    && y - 1 >= 0
                    && field[x + dx, y, z + dz] == Element.Empty
                    && field[x + dx, y - 1, z + dz] == Element.Empty)
                {
                    int next_y = y;
                    while (next_y >= 0 && field[x + dx, next_y, z + dz] == 0) next_y--;
                    GameObject tmp = Instantiate(hintPrefab, new Vector3(x + dx, next_y, z + dz), Quaternion.identity);
                    //print("fall hint: (" + (x + dx) + ", " + (next_y) + ", " + (z + dz) + ")");
                    hintQueue.Add(tmp);
                }
            }
        }

        if (!isMoving && isKangaroo)
        {
            // jump 2 levels
            dx = 0; dy = 0; dz = 0; // 歸零
            for (int i = 0; i < 4; i++)
            {
                dx = offset[i, 0]; dz = offset[i, 1];
                if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                    && y + 1 < y_max
                    && y + 2 < y_max
                    && (field[x + dx, y, z + dz] == Element.Cube || field[x + dx, y, z + dz] == Element.Rock)
                    && (field[x + dx, y + 1, z + dz] == Element.Cube || field[x + dx, y + 1, z + dz] == Element.Rock)
                    && y + 2 < y_max && field[x + dx, y + 2, z + dz] == Element.Empty)
                {
                    GameObject tmp = Instantiate(hintPrefab, new Vector3(x + dx, y + 1, z + dz), Quaternion.identity);
                    //print("jump 2 level hint: (" + (x + dx) + ", " + (y + 1) + ", " + (z + dz) + ")");
                    hintQueue.Add(tmp);
                }
            }
        }

        if (!isMoving && isBear)
        {
            // rock
            dx = 0; dy = 0; dz = 0; // 歸零
            for (int i = 0; i < 4; i++)
            {
                dx = offset[i, 0]; dz = offset[i, 1];
                dy = 0;
                float rock_dy = 0;
                if (x + dx >= 0 && x + dx < x_max && z + dz >= 0 && z + dz < z_max
                    && x + dx * 2 >= 0 && x + dx * 2 < x_max && z + dz * 2 >= 0 && z + dz * 2 < z_max
                    && y - 1 >= 0
                    && field[x + dx, y, z + dz] == Element.Rock
                    && field[x + dx * 2, y, z + dz * 2] == Element.Empty
                    )
                {
                    GameObject tmp = Instantiate(rockHintPrefab, new Vector3(x + dx, y, z + dz), Quaternion.identity);
                    //print("rock hint: (" + (x + dx) + ", " + (y) + ", " + (z + dz) + ")");
                    hintQueue.Add(tmp);

                }

            }
        }






    }
    public void getMovingInfo()
    {
        Ray ray;
        RaycastHit hit;
        Camera playerCamera = Camera.main;
        LayerMask mask;

        mask = 1 << 6;
        ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                Vector3 hintPos;
                if (hit.transform.name == "rockHintPrefab(Clone)")
                {
                    hintPos = hit.transform.position;
                    isMovingRock = true;
                }
                else
                {
                    hintPos = hit.transform.position + new Vector3(0f, 0.5f, 0f);
                }

                if (hintPos.x - x == 1.0f)
                {
                    movingDir = 0;
                    print("mouseHint: " + movingDir);
                }
                if (hintPos.x - x == -1.0f)
                {
                    movingDir = 1;
                    print("mouseHint: " + movingDir);
                }
                if (hintPos.z - z == 1.0f)
                {
                    movingDir = 2;
                    print("mouseHint: " + movingDir);
                }
                if (hintPos.z - z == -1.0f)
                {
                    movingDir = 3;
                    print("mouseHint: " + movingDir);
                }
            }
        }
    }
    public void resetMovingInfo()
    {
        movingDir = -1;
        isMovingRock = false;
    }
    public void detactPointing()
    {
        Ray ray;
        RaycastHit hit;
        Camera playerCamera = Camera.main;
        LayerMask mask;

        mask = 1 << 6;
        ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            print(hit.transform.name);
            if (hit.transform.tag == "Hint")
            {
                Renderer renderer = hit.transform.gameObject.GetComponent<Renderer>();
                renderer.material = pointingHintMat;
                print("Hint");
            }
        }
    }

    void Update()
    {
        nextStepHint();
        getMovingInfo();
        detactPointing();
        if (getMoveKey() && (Input.GetKey(KeyCode.O) || isMovingRock))
        {
           
            if (move_rock())
            {
                audioSource.clip = specialSound;
                audioSource.Play();
                print("move_rock");
                Text.text = "step:" + step.ToString();
            }
        }
        else if (getMoveKey())
        {
            
            print("MoveMoveMove");
            if (move())
            {
                audioSource.clip = moveSound;
                audioSource.Play();
                print("move");
                Text.text = "step:" + step.ToString();
            }
            else if (jump_1_level())
            {
                audioSource.clip = moveSound;
                audioSource.Play();
                print("jump_1_level");
                Text.text = "step:" + step.ToString();
            }
            else if (jump_2_level())
            {
                audioSource.clip = specialSound;
                audioSource.Play();
                print("jump_2_level");
                Text.text = "step:" + step.ToString();
                Jump2Text.text = "jump_2:" + (Jump2MaxNum - jump2_num).ToString();

            }
            else if (fall())
            {
                print("fall");
                Text.text = "step:" + step.ToString();
            }

        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            getMovingInfo();
            if (poop())
            {
                print("poop");
                poopText.text = "poop:" + (poopMaxNum - poop_num).ToString();
            }

        }
        resetMovingInfo();
        detectSave();
        detectWin();
        deteceGameover();
    }
}
